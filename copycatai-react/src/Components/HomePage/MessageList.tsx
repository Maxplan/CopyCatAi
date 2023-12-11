import React from "react";
import MessageItem from "./MessageItem";

interface MessageListProps {
    messages: {
        role: "user" | "assistant"
        content: string | JSX.Element[]
    }[]
}


const MessageList: React.FC<MessageListProps> = ({ messages }) => {
  return (
    <div className="messageList">
        {messages.map((msg, index) => {
            return <MessageItem key={index} message={msg} />;
        })}
    </div>
  );
};

export default MessageList;