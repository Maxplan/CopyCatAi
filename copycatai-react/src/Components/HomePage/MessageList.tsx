import React from "react";
import MessageItem from "./MessageItem";

interface MessageListProps {
  messages: {
    role: "user" | "assistant";
    content: string | JSX.Element[];
    responseId?: number;
    originalUserRequest?: string;
    userRating?: 'like' | 'dislike' | null;
    conversationId: number;
  }[],
  onResendRequest: (request: string) => void;
}

const MessageList: React.FC<MessageListProps> = ({ messages, onResendRequest }) => {
  return (
    <div className="messageList">
      {messages.map((msg, index) => (
        <MessageItem 
          key={index} 
          message={msg}
          onResendRequest={onResendRequest}
        />
      ))}
    </div>
  );
};

export default MessageList;
