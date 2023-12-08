interface Conversation {
    conversationId: number;
    timestamp: string;
    requests: string[];
    responses: string[];
}

export default Conversation;