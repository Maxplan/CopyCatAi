// Purpose: Show historic conversations and allow user to select one to view

import React, { useState, useEffect } from "react";
import Conversation from "../Shared/ConversationTypes";
import newConversationIcon from "../../imgs/new-conversation.png";

import "../../Styles/sideBar.css";

interface SidebarProps {
    authToken: string;
    userId: string;
    onConversationSelect: (conversationId: number) => Promise<void>;
    onStartNewConversation: () => void;
}

const truncateString = (str: string, num: number) => {
    if (str.length <= num) {
        return str
    }
    return str.slice(0, num) + '...'
}

const Sidebar: React.FC<SidebarProps> = ({ authToken, userId, onConversationSelect, onStartNewConversation }) => {
    const [conversations, setConversations] = useState<Conversation[]>([]);

    useEffect(() => {
        const fetchConversations = async () => {
            const response = await fetch(`http://localhost:5119/api/v1/Interaction/GetConversationByUserId`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${authToken}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                setConversations(data);
            }
        };

        if (userId) {
            fetchConversations();
        }
    }, [userId, authToken]);

    return (
    <div className="sidebar">
        <div className="sidebar-header">
            <h3>History</h3>
            <button className="new-conversation-btn" onClick={onStartNewConversation}>
                <img className="new-conversation-icon" src={newConversationIcon} alt="" />
            </button>
        </div>
        <ul>
            {conversations.map((conversation, index) => (
                <li key={index} onClick={() => onConversationSelect(conversation.conversationId)}>
                    {/* Check if RequestPrompt is available, else show Request */}
                    {truncateString(
                        conversation.requestPrompts[0] ? conversation.requestPrompts[0] : conversation.requests[0],
                        15
                    )}
                </li>
            ))}
        </ul>
    </div>
);
};

export default Sidebar;