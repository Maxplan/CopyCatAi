import React, { useEffect, useState } from 'react';
import thumbsUpIcon from '../../imgs/thumbs-up.svg';
import thumbsDownIcon from '../../imgs/thumbs-down.svg';
import reloadIcon from '../../imgs/reload.svg';

interface MessageItemProps {
  message: {
    role: "user" | "assistant";
    content: string | JSX.Element[];
    responseId?: number;
    userRating?: 'like' | 'dislike' | null;
    conversationId: number;
  };
  originalUserRequest?: string;
  onResendRequest: (request: string) => void;
}


const MessageItem: React.FC<MessageItemProps> = ({ message, originalUserRequest, onResendRequest }) => {
  const [selectedRating, setSelectedRating] = useState<'like' | 'dislike' | null>(message.userRating || null);
  const authToken = localStorage.getItem('token') || "";

  useEffect(() => {
  setSelectedRating(message.userRating ?? null);
}, [message.conversationId, message.userRating]);

  const handleRatingClick = async (rating: 'like' | 'dislike', responseId: number) => {
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
        if (originalUserRequest) {
            onResendRequest(originalUserRequest);
        }
    };

    if (message.role === 'assistant') {
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
        return <p className="user-message">{message.content}</p>;
    }
};

export default MessageItem;
