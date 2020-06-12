using Microsoft.EntityFrameworkCore;

namespace EmailJob.DataModel
{
    public class EmailContext : DbContext {
        public EmailContext () {

        }
        public EmailContext (DbContextOptions<EmailContext> option) : base (option) {

        }

        public DbSet<Email> Email { get; set; }
        

        protected override void OnConfiguring (DbContextOptionsBuilder options) {
            if (!options.IsConfigured) {
                options.UseSqlServer ("");
            }
        }
    }
}