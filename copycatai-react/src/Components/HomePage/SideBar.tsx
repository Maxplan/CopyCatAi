// Purpose: Show historic conversations and allow user to select one to view

import React, { useState, useEffect } from "react";
import Conversation from "../Shared/ConversationTypes";

import "../../Styles/sideBar.css";

interface SidebarProps {
    authToken: string;
    userId: string;
    onConversationSelect: (conversationId: number) => Promise<void>;
}

const truncateString = (str: string, num: number) => {
    if (str.length <= num) {
        return str
    }
    return str.slice(0, num) + '...'
}

const Sidebar: React.FC<SidebarProps> = ({ authToken, userId, onConversationSelect }) => {
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
            <h3>History</h3>
            <ul>
                {conversations.map((conversation, index) => (
                    <li key={index} onClick={() => onConversationSelect(conversation.conversationId)}>
                        {truncateString(conversation.requests[0], 15)}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Sidebar;