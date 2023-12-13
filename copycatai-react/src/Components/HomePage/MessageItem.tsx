import React from 'react';
import thumbsUpIcon from '../../imgs/thumbs-up.svg';
import thumbsDownIcon from '../../imgs/thumbs-down.svg'
import { useState } from 'react';

interface MessageItemProps {
    message: {
      role: "user" | "assistant";
      content: string | JSX.Element[];
    }
}

const MessageItem: React.FC<MessageItemProps> = ({ message }) => {

  const [selectedRating, setSelectedRating] = useState<string | null>(null);

  const handleRatingClick = (rating: string) => {
    setSelectedRating(rating); // 'like' or 'dislike'
    // Optionally, send the rating to your API here
  };

  if (message.role === 'assistant') {
    // AI message: wrapped in a div with additional rating buttons
    return (
      <div className="response-div">
        <p className="ai message">{message.content}</p>
        <div className="rating-bar">
          <button
            onClick={() => handleRatingClick('like')}
            className={selectedRating === 'like' ? 'selected' : ''}
          >
            <img src={thumbsUpIcon} alt="Like" />
          </button>
          <button
            onClick={() => handleRatingClick('dislike')}
            className={selectedRating === 'dislike' ? 'selected' : ''}
          >
            <img src={thumbsDownIcon} alt="Dislike" />
          </button>
        </div>
      </div>
    );
  } else {
    // User message: just the paragraph
    return <p className="user-message">{message.content}</p>;
  }
};

export default MessageItem;
