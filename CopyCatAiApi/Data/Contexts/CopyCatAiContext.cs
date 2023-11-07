using CopyCatAiApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CopyCatAiApi.Data.Contexts
{
    public class CopyCatAiContext : IdentityDbContext<UserModel>
    {
        // Tables
        public DbSet<ConversationModel> Conversations { get; set; }
        public DbSet<RequestModel> Requests { get; set; }
        public DbSet<ResponseModel> Responses { get; set; }
        public DbSet<SettingsModel> Settings { get; set; }
        
        // Constructor using dependency injection
        public CopyCatAiContext(DbContextOptions options) : base(options) { }

        // Override OnModelCreating to configure the database

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserModel
             modelBuilder.Entity<UserModel>()
                .HasMany(u => u.ConversationList)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<SettingsModel>(s => s.UserId)
                .IsRequired();

            // ConversationModel
            modelBuilder.Entity<ConversationModel>()
                .HasMany(c => c.RequestList)
                .WithOne(r => r.Conversation)
                .HasForeignKey(r => r.ConversationId)
                .IsRequired();

            modelBuilder.Entity<ConversationModel>()
            .HasMany(c => c.ResponseList)
            .WithOne(r => r.Conversation)
            .HasForeignKey(r => r.ConversationId)
            .IsRequired();


            
        }
    }
}