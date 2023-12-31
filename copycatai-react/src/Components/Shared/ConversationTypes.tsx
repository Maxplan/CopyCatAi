interface Conversation {
    conversationId: number;
    timestamp: string;
    requests: string[];
    requestPrompts: string[];
    responses: Array<{
        response: string;
        responseId: number;
    }>;
}

export default Conversation;