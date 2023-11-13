import React from "react";
import MessageItem from "./MessageItem";

interface MessageListProps {
    messages: {
        type: "user" | "assistant"
        text: string
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