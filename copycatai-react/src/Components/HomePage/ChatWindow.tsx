import React, { useEffect, useState } from 'react';
import MessageList from './MessageList';
import InputBar from './InputBar';
import '../../Styles/chatWindow.css';
import { formatResponse } from '../Shared/FormattingService';

// Define the structure of a message
interface Message {
  type: 'user' | 'assistant';
  text: string | JSX.Element[];
}

const ChatWindow: React.FC = () => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [conversationId, setConversationId] = useState<number | null>(null);

    const authToken = localStorage.getItem('token')
    
    useEffect(() => {
        let isMounted = true;
        const fetchConversationId = async () => {

            if (isMounted){
                const response = await fetch('http://localhost:5119/api/v1/Interaction/StartConversation', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${authToken}`
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    setConversationId(data.conversationId);
            }}
        };
        fetchConversationId();
        return () => { isMounted = false };
    }, [authToken]);

    const sendMessageToApi = async (conversation: Message[]) => {
        try {
            const formattedConversation = conversation.map(msg => ({
                Role: msg.type, // Ensure these field names match the ChatMessage class in your API
                Content: typeof msg.text === 'string' ? msg.text : msg.text.join(''),
            }));
        
            console.log("Sending message to API: ", formattedConversation);
            const response = await fetch('http://localhost:5119/api/v1/Interaction/Sendmessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authToken}`
                },
                body: JSON.stringify({ conversation: formattedConversation, conversationId })
            });
        
            if (!response.ok) {
                const errorResponse = await response.text();
                console.error("Error response from API: ", errorResponse);
                throw new Error('Error in sending message to API');
            }
        
            const data = await response.text();
            return data;
        } catch (error) {
            console.error("Failed to send message: ", error);
            return "Error in sending message";
        }
    };

    const handleSendMessage = async (messageText: string) => {
        const newUserMessage: Message = { type: 'user', text: messageText };
        const loadingMessage: Message = { type: 'assistant', text: '...' };

        setMessages(currentMessages => [...currentMessages, newUserMessage, loadingMessage]);

        try {
            const fullConversation = messages.concat(newUserMessage);
            const apiResponse = await sendMessageToApi(fullConversation);

            // Parse the JSON response
            const parsedResponse = JSON.parse(apiResponse);

            // Extract the 'response' field from the parsed response
            const textResponse = parsedResponse.response;

            // Update the conversation ID if provided in the response
            if (parsedResponse.conversationId) {
                setConversationId(parsedResponse.conversationId);
            }

            const formattedTextResponse = formatResponse(textResponse || "Error in sending message");

            setMessages(currentMessages => {
                const messagesWithoutLoading = currentMessages.slice(0, -1);
                return [...messagesWithoutLoading, { type: 'assistant', text: formattedTextResponse }];
            });
        } catch (error) {
            setMessages(currentMessages => {
                const messagesWithoutLoading = currentMessages.slice(0, -1);
                return [...messagesWithoutLoading, { type: 'assistant', text: "Error in sending message" }];
            });
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
