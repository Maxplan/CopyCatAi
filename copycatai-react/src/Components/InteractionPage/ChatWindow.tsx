import React, { useState } from 'react';
import MessageList from './MessageList';
import InputBar from './InputBar';

// Define the structure of a message
interface Message {
  type: 'user' | 'assistant';
  text: string;
}

const ChatWindow: React.FC = () => {
    const [messages, setMessages] = useState<Message[]>([]);

    const sendMessageToApi = async (conversation: Message[]) => {
        try {

            const formattedConversation = conversation.map(msg => ({
            Role: msg.type, // Mapping 'type' to 'Role'
            Content: msg.text // Mapping 'text' to 'Content'
        }));

            console.log("Sending message to API: ", conversation)
            const response = await fetch('http://localhost:5119/api/v1/Interaction/Sendmessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify( formattedConversation )
            });
            if (!response.ok) {
                throw new Error('Error in sending message to API');
            }
            const data = await response.text();
            return data;
        } catch (error) {
            console.error("Failed to send message: ", error);
            return "Error in sending message"
        }
    
    };

    const handleSendMessage = async (messageText: string) => {
        const newUserMessage: Message = { type: 'user', text: messageText };
        const loadingMessage: Message = { type: 'assistant', text: '...' };

        setMessages(currentMessages => [...currentMessages, newUserMessage, loadingMessage]);

        try {
            const fullConversation = messages.concat(newUserMessage);
            const apiResponse = await sendMessageToApi(fullConversation);

            setMessages(currentMessages => {
                const messagesWithoutLoading = currentMessages.slice(0, -1);
                return [...messagesWithoutLoading, { type: 'assistant', text: apiResponse || "Error in sending message" }];
            })
        } catch (error) {
            setMessages(currentMessages => {
                const messagesWithoutLoading = currentMessages.slice(0, -1);
                return [...messagesWithoutLoading, { type: 'assistant', text: "Error in sending message" }];
            });
        }
    };

    return (
        <div>
            <MessageList messages={messages} />
            <InputBar onSendMessage={handleSendMessage} />
        </div>
  );
};

export default ChatWindow;
