using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EmailJob.Services
{
    public interface IEmailClient
    {
        Task Send(string from, string to, string subject, string body);   
    }
    
    //Email client to send email
    public class EmailClient : IEmailClient
    {
        private readonly SmtpClient smtpClient;
        
        EmailClient(IConfiguration config)
        {
            smtpClient = new SmtpClient(config.GetConnectionString("EmailClient"));
        }

        Task IEmailClient.Send(string from, string to, string subject, string body)
        {
            smtpClient.Send(from, to, subject, body);
            return null;
        }
    }
}