interface Conversation {
    conversationId: number;
    timestamp: string;
    requests: string[];
    responses: Array<{
        response: string;
        responseId: number;
    }>;
}

export default Conversation;