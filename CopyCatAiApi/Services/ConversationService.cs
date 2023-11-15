// Purpose: To provide a service for saving conversations to the database.
using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task SaveRequestToDatabase(RequestModel request)
        {
            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task SaveResponseToDatabase(ResponseModel response)
        {
            await _context.Responses.AddAsync(response);
            await _context.SaveChangesAsync();
        }

        public async Task SaveConversationToDatabase(ConversationModel conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }
    }
}