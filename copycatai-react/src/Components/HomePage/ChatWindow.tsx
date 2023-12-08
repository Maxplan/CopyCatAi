import React, { useEffect, useState } from 'react';
import MessageList from './MessageList';
import InputBar from './InputBar';
import '../../Styles/chatWindow.css';
import { formatResponse } from '../Shared/FormattingService';
import Conversation from '../Shared/ConversationTypes';

// Define the structure of a message
interface Message {
  type: 'user' | 'assistant';
  text: string | JSX.Element[];
}

interface ChatWindowProps {
    selectedConversation: Conversation | null
}

const ChatWindow: React.FC<ChatWindowProps> = ({ selectedConversation}) => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [conversationId, setConversationId] = useState<number | null>(null);

    const authToken = localStorage.getItem('token');
    
    const interleaveArrays = (arr1: string[], arr2: string[]): Message[] => {
        const result: Message[] = [];
        const length = Math.max(arr1.length, arr2.length);
        
        for (let i = 0; i < length; i++) {
            if (i < arr1.length) {
                result.push({ type: 'user', text: arr1[i] });
            }
            if (i < arr2.length) {
                result.push({ type: 'assistant', text: arr2[i] });
            }
    }

    return result;
    }

    useEffect(() => {
        let isMounted = true;

        const loadSelectedConversation = () => {
            if (selectedConversation) {
                setConversationId(selectedConversation.conversationId);

                // Use the interleaveArrays function to combine requests and responses
                const combinedMessages = interleaveArrays(
                    selectedConversation.requests,
                    selectedConversation.responses
                );

                console.log("Loaded Conversation: ", combinedMessages);
                setMessages(combinedMessages);
            }
        };

        const startNewConversation = async () => {
            if (!selectedConversation && isMounted) {
                const response = await fetch('http://localhost:5119/api/v1/Interaction/StartConversation', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${authToken}`
                    }
                });

                if (response.ok) {
                    const data = await response.json();
                    setConversationId(data.conversationId);
                    console.log("Start new conversation: ", data.conversationId)
                }
            }
        };

        // Call the appropriate function based on whether a conversation is selected
        if (selectedConversation) {
            loadSelectedConversation();
        } else {
            startNewConversation();
        }

        return () => { isMounted = false };
    }, [selectedConversation, authToken]);

    const sendTextMessageToApi = async (conversation: Message[]) => {
        try {
            const formattedConversation = conversation.map(msg => ({
                Role: msg.type,
                Content: typeof msg.text === 'string' ? msg.text : msg.text.join(''),
            }));

            const response = await fetch('http://localhost:5119/api/v1/Interaction/Sendmessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authToken}`
                },
                body: JSON.stringify({ conversation: formattedConversation, conversationId })
            });

            const data = await response.json();
            return data;
        } catch (error) {
            console.error("Failed to send message: ", error);
            return "Error in sending message";
        }
    };

    const sendPdfMessageToApi = async (conversation: Message[], file: File) => {
        try {
            const formData = new FormData();
            formData.append('conversation', JSON.stringify({ conversation, conversationId }));
            formData.append('pdfFile', file);

            const response = await fetch('http://localhost:5119/api/v1/Interaction/SendPdfMessage', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${authToken}`
                },
                body: formData
            });
            
            const data = await response.json();
            return data;
            // ...handle the response
        } catch (error) {
            // ...error handling
        }
    };

    const handleSendMessage = async (messageText: string, file?: File) => {
        const newUserMessage: Message = { type: 'user', text: messageText };
        const loadingMessage: Message = { type: 'assistant', text: '...' };
        setMessages(currentMessages => [...currentMessages, newUserMessage, loadingMessage]);

        const fullConversation = messages.concat(newUserMessage);

        console.log("fullConversation: ", fullConversation)
        let apiResponse;

        if (file) {
            // Handle sending message with PDF file
            apiResponse = await sendPdfMessageToApi(fullConversation, file);
            console.log("apiResponsePdf: ", apiResponse)
        } else {
            // Handle sending regular text message
            apiResponse = await sendTextMessageToApi(fullConversation);
            console.log("apiResponseText: ", apiResponse)
        }

        // Remove the loading message
        setMessages(currentMessages => currentMessages.slice(0, -1));

        if (apiResponse) {
            const textResponse = apiResponse.response || "Error in sending message";
            console.log("textResponse: ", textResponse)
            const formattedTextResponse = formatResponse(textResponse);
            console.log(formattedTextResponse)

            // Update the messages state with the new response
            setMessages(currentMessages => [...currentMessages, { type: 'assistant', text: formattedTextResponse }]);
            console.log("messages: ", messages)
        } else {
            // Handle error in response
            setMessages(currentMessages => [...currentMessages, { type: 'assistant', text: "Error in sending message" }]);
            console.log("messages: ", messages)
        }
    };

    return (
        <div className="chat-container">
            <MessageList messages={messages} />
            <InputBar onSendMessage={handleSendMessage} />
        </div>
    );
};

export default ChatWindow;
