// Purpose: To provide a service for handling conversation data.
using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CopyCatAiApi.Services
{
    public class ConversationService
    {
        private readonly CopyCatAiContext _context;

        public ConversationService(CopyCatAiContext context)
        {
            _context = context;
        }

        //Save Methods
        public async Task SaveRequestToDatabase(RequestModel request)
        {
            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task SaveResponseToDatabase(ResponseModel response)
        {
            var existingResponse = await _context.Responses.FindAsync(response.ResponseId);
            if (existingResponse != null)
            {
                _context.Entry(existingResponse).CurrentValues.SetValues(response);
            }
            else
            {
                await _context.Responses.AddAsync(response);
            }
            await _context.SaveChangesAsync();
        }

        public async Task SavePromptToDatabase(int requestId, string prompt)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null)
            {
                throw new Exception("No request found with this id.");
            }
            request.RequestPrompt = prompt;
            await _context.SaveChangesAsync();
        }
        public async Task SaveConversationToDatabase(ConversationModel conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }

        //Get Methods
        public async Task<List<ConversationModel>> GetConversationsByUserId(string userId)
        {
            var result = await _context.Conversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Timestamp)
                .ToListAsync() ?? throw new Exception("No conversations found for this user.");

            return result;
        }

        public async Task<List<RequestModel>> GetRequestsByConversationId(int conversationId)
        {
            var result = await _context.Requests
                .Where(r => r.ConversationId == conversationId)
                .OrderBy(r => r.TimeStamp)
                .ToListAsync() ?? throw new Exception("No requests found for this conversation.");

            return result;
        }

        public async Task<List<string>> GetRequestPromptsByConversationId(int conversationId)
        {
            var result = await _context.Requests
                .Where(r => r.ConversationId == conversationId)
                .OrderBy(r => r.TimeStamp)
                .Select(r => r.RequestPrompt)
                .ToListAsync() ?? throw new Exception("No requests found for this conversation.");

            return result;
        }
        public async Task<List<ResponseModel>> GetResponsesByConversationId(int conversationId)
        {
            var result = await _context.Responses
                .Where(r => r.ConversationId == conversationId)
                .OrderBy(r => r.TimeStamp)
                .ToListAsync() ?? throw new Exception("No responses found for this conversation.");

            return result;
        }

        public async Task<List<RequestModel>> GetRequestsByUserId(string userId)
        {
            var result = await _context.Requests
                .Where(r => r.Conversation!.UserId == userId)
                .OrderBy(r => r.TimeStamp)
                .ToListAsync() ?? throw new Exception("No requests found for this user.");

            return result;
        }

        public async Task<List<ResponseModel>> GetResponsesByUserId(string userId)
        {
            var result = await _context.Responses
                .Where(r => r.Conversation!.UserId == userId)
                .OrderBy(r => r.TimeStamp)
                .ToListAsync() ?? throw new Exception("No responses found for this user.");

            return result;
        }

        public async Task<RequestModel> GetRequestById(int requestId)
        {
            var result = await _context.Requests
                .Where(r => r.RequestId == requestId)
                .FirstOrDefaultAsync() ?? throw new Exception("No request found with this id.");

            return result;
        }

        public async Task<ResponseModel> GetResponseById(int responseId)
        {
            var result = await _context.Responses
                .Where(r => r.ResponseId == responseId)
                .FirstOrDefaultAsync() ?? throw new Exception("No response found with this id.");

            return result;
        }

        public async Task<ConversationModel> GetConversationById(int conversationId)
        {
            var result = await _context.Conversations
                .Where(c => c.ConversationId == conversationId)
                .FirstOrDefaultAsync() ?? throw new Exception("No conversation found with this id.");

            var req = result.RequestList = await GetRequestsByConversationId(conversationId);

            return result;
        }

        //Delete Methods
        public async Task DeleteConversationById(int conversationId)
        {
            var conversation = await GetConversationById(conversationId);

            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
        }
    }
}