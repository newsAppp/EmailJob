using System;

namespace EmailJob.DataModel {
    public enum EmailStatus { New, Sent, Opened }
    public enum MailTemplate { Welcome, ThanksYou, Reminder }
    public class Email {
        public Guid EmailId { get; set; }
        public string From { get; set; }
        //comma seprated email ids
        public string To { get; set; }
        public string Content { get; set; }
        public EmailStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}