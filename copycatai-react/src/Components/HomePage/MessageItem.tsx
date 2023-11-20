import React from 'react';

interface MessageItemProps {
    message: {
      type: "user" | "assistant";
      text: string | JSX.Element[];
    }
}

const MessageItem: React.FC<MessageItemProps> = ({ message }) => {
  const { type, text } = message;
  const messageClass = type === 'user' ? 'user-message' : 'ai-message';

  return (
    <p className={messageClass}>{text}</p>
  );
};

export default MessageItem;
