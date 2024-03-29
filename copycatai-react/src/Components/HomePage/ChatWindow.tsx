import React, { useEffect, useState } from 'react';
import MessageList from './MessageList';
import InputBar from './InputBar';
import '../../Styles/chatWindow.css';
import { formatResponse } from '../Shared/FormattingService';
import Conversation from '../Shared/ConversationTypes';

// Define the structure of a message
interface Message {
  role: 'user' | 'assistant';
  content: string | JSX.Element[];
  responseId?: number;
  originalUserRequest?: string;
  userRating?: 'like' | 'dislike' | null;
  conversationId: number;
}

interface ChatWindowProps {
    selectedConversation: Conversation | null
}

const ChatWindow: React.FC<ChatWindowProps> = ({ selectedConversation }) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [conversationId, setConversationId] = useState<number | null>(null);
  const authToken = localStorage.getItem('token');
    
    const interleaveArrays = (arr1: string[], arr2: string[], responseIds: number[], requestPrompts: string[], conversationId: number): Message[] => {
        const result: Message[] = [];
        const length = Math.max(arr1.length, arr2.length);
        
        for (let i = 0; i < length; i++) {
          if (i < arr1.length) {
              const responseContent = requestPrompts[i] || arr1[i];
              const formattedRequest = formatResponse(responseContent);
                result.push({ role: 'user', content: formattedRequest, conversationId});
            }
            if (i < arr2.length) {
              const formattedResponse = formatResponse(arr2[i]);
              const responseId = responseIds[i];
                result.push({ role: 'assistant', content: formattedResponse, responseId, conversationId });
            }
    }

    return result;
  }
  
  const handleResendRequest = async (request: string) => {
    const message: Message = { role: 'user', content: request, conversationId: conversationId! };
    await handleSendMessage(message);
  }

    useEffect(() => {
      let isMounted = true;
      
      const loadSelectedConversation = () => {
          if (selectedConversation) {
              setConversationId(selectedConversation.conversationId);

              const responseIds = selectedConversation.responses.map(response => response.responseId);
            
              const combinedMessages = interleaveArrays(
                selectedConversation.requests,
                selectedConversation.responses.map(response => response.response),
                responseIds,
                selectedConversation.requestPrompts,
                selectedConversation.conversationId
              );
              
              console.log("Loaded Conversation: ", combinedMessages);
            setMessages(combinedMessages);
          } else {
            setMessages([])
            setConversationId(null);
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
                  setMessages([]); // Clear messages for a new conversation
                  console.log("Start new conversation: ", data.conversationId)
              }
          }
      };
    
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
                Role: msg.role,
                Content: typeof msg.content === 'string' ? msg.content : msg.content.join(''),
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
        formData.append('pdfFile', file);
        formData.append('conversationId', conversationId!.toString());

        conversation.forEach((msg, index) => {
            formData.append(`conversation[${index}][role]`, msg.role);
            formData.append(`conversation[${index}][content]`, convertContentToPlainText(msg.content));
        });

        // Alternative debugging: Log FormData contents
        formData.forEach((value, key) => {
            console.log(`${key}: ${value}`);
        });

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

    const convertContentToPlainText = (content: string | JSX.Element[]): string => {
      if (typeof content === 'string') {
        return content;
      } else if (Array.isArray(content)) {
        return content.map(item => {
          if (typeof item.props.children === 'string') {
            return item.props.children;
          } else if (Array.isArray(item.props.children)) {
            // Providing an explicit type for 'child'
            return item.props.children.filter((child: React.ReactNode) => typeof child === 'string').join(' ');
          }
          return '';
        }).join(' ');
      }
      return ''; // Default fallback
    };

    const handleSendMessage = async (newMessage: Message, file?: File,) => {
      // Convert existing messages content to plain text
      const currentConversationId = conversationId || -1;

      const updatedMessages = messages.map(msg => ({
        role: msg.role,
        content: convertContentToPlainText(msg.content),
        conversationId: msg.conversationId // Add this line
      }));
    
      // Convert new message content to plain text and add to messages
      updatedMessages.push({
        role: newMessage.role,
        content: convertContentToPlainText(newMessage.content),
        conversationId: newMessage.conversationId || currentConversationId // Add this line
      });
    
      setMessages(currentMessages => [...currentMessages, newMessage]);
    
      // Prepare payload for API
      const payload = {
        conversation: updatedMessages,
        conversationId: conversationId
      };
    
      // Log payload for debugging
      console.log("Sending payload:", payload);
    
      let apiResponse;
      let textResponse;
    
      if (file) {
        apiResponse = await sendPdfMessageToApi(updatedMessages, file);
      } else {
        apiResponse = await sendTextMessageToApi(updatedMessages);
      }
    
      // Handle the response from the API
      if (apiResponse) {
        textResponse = apiResponse.response || "Error in sending message";
        const responseMessage: Message = {
          role: 'assistant',
          content: formatResponse(textResponse),
          conversationId: conversationId || currentConversationId // Add this line
        };
        setMessages(currentMessages => [...currentMessages, responseMessage]);
      } else {
        // Handle error in response
        const errorMessage: Message = {
          role: 'assistant',
          content: "Error in sending message",
          conversationId: conversationId || currentConversationId // Add this line
        };
        setMessages(currentMessages => [...currentMessages, errorMessage]);
      }
    };



    return (
    <div className="chat-container">
      <MessageList messages={messages} onResendRequest={handleResendRequest} />
      <InputBar onSendMessage={handleSendMessage} conversationId={conversationId} />
    </div>
  );
};

export default ChatWindow;
