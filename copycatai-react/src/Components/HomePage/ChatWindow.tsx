import React, { useEffect, useState } from 'react';
import MessageList from './MessageList';
import InputBar from './InputBar';
import '../../Styles/chatWindow.css';
import { formatResponse } from '../Shared/FormattingService';

// Define the structure of a message
interface Message {
  type: 'user' | 'assistant';
  text: string;
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
        // Format the conversation for the API request
        const formattedConversation = conversation.map(msg => ({
            Role: msg.type,
            Content: msg.text
        }));

        console.log("Sending message to API: ", formattedConversation); // Debugging line

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

            const data = await response.json();
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

            const textResponse = apiResponse.response || "Error in sending message";

            setMessages(currentMessages => {
                const messagesWithoutLoading = currentMessages.slice(0, -1);
                return [...messagesWithoutLoading, { type: 'assistant', text: textResponse }];
            });

            if (apiResponse.conversationId) {
                setConversationId(apiResponse.conversationId);
            }
        } catch (error) {
            setMessages(currentMessages => {
                const messagesWithoutLoading = currentMessages.slice(0, -1);
                return [...messagesWithoutLoading, { type: 'assistant', text: "Error in sending message" }];
            });
        }
    };

    return (
        <div className="chat-container">
            <MessageList messages={messages.map(msg => ({
                ...msg,
                text: msg.type === 'assistant' ? formatResponse(msg.text) : msg.text
            }))} />
            <InputBar onSendMessage={handleSendMessage} />
        </div>
    );
};

export default ChatWindow;
