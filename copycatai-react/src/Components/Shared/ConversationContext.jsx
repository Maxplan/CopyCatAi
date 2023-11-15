import React, { createContext, useState } from 'react';

export const ConversationContext = createContext();

export const ConversationProvider = ({ children }) => {
    const [conversationId, setConversationId] = useState(null);

    return (
        <ConversationContext.Provider value={{ conversationId, setConversationId }}>
            {children}
        </ConversationContext.Provider>
    );
};
