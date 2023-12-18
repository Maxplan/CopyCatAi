import React from 'react';
import thumbsUpIcon from '../../imgs/thumbs-up.svg';
import thumbsDownIcon from '../../imgs/thumbs-down.svg'
import reloadIcon from '../../imgs/reload.svg';
import { useState } from 'react';

interface MessageItemProps {
    message: {
      role: "user" | "assistant";
      content: string | JSX.Element[];
      responseId?: number;
    };
    originalUserRequest?: string; // Add this
    onResendRequest: (request: string) => void; // Add this
}

const MessageItem: React.FC<MessageItemProps> = ({ message, originalUserRequest, onResendRequest }) => {

  const [selectedRating, setSelectedRating] = useState<string | null>(null);
  const authToken = localStorage.getItem('token') || "";

  const handleRatingClick = async (rating: string, responseId: number) => {
    setSelectedRating(rating); 
    const isLike = rating === 'like';

    try {
      const response = await fetch(`http://localhost:5119/api/v1/Interaction/RateResponse`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authToken}`
        },
        body: JSON.stringify({
          responseId: responseId,
          rating: isLike
        })
      });

      if (response.ok) {
        console.log('Rating submitted successfully');
      } else {
        console.error('Rating submission failed');
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleReloadClick = () => {
    if(originalUserRequest) {
      onResendRequest(originalUserRequest);
    }
  };

  if (message.role === 'assistant' && message.responseId) {
    // AI message: wrapped in a div with additional rating buttons
    return (
      <div className="response-div">
        <p className="ai message">{message.content}</p>
        <div className="rating-bar">
          <button onClick={handleReloadClick}>
            <img src={reloadIcon} alt="reload" />
          </button>
          <button
            onClick={() => handleRatingClick('like', message.responseId!)}
            className={selectedRating === 'like' ? 'selected' : ''}
          >
            <img src={thumbsUpIcon} alt="Like" />
          </button>
          <button
            onClick={() => handleRatingClick('dislike', message.responseId!)}
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
