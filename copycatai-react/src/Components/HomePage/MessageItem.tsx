import React from 'react';

interface MessageItemProps {
    message: {
      role: "user" | "assistant";
      content: string | JSX.Element[];
    }
}

const MessageItem: React.FC<MessageItemProps> = ({ message }) => {
  const { role, content } = message;
  const messageClass = role === 'user' ? 'user-message' : 'ai-message';

  return (
    <p className={messageClass}>{content}</p>
  );
};

export default MessageItem;
