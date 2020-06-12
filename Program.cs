using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using EmailJob.DataModel;
using EmailJob.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailJob {
    class Program {

        //connection string for Email data base
        public static string ConnectionString { get; private set; }
        //freq of repeating the job, in present scenerio its every day
        private static double interval;
        //time when job executes, in present scenerio its 12:00 AM
        private static int hours, min;

        static void Main (string[] args) {
            SchedulerService.Instance.ScheduleTask (hours, min, interval * 24, ProcessMail);
        }

        //Process Mail as mention in defined problem.
        public static void ProcessMail () {
            IServiceProvider servicesProvider = BuildDi (); 
            using (servicesProvider as IDisposable) {
                var emailClient = servicesProvider.GetService<IEmailClient> ();
                using (var emailContext = new EmailContext ()) {
                    //step 1: sending "Thank you" to all opened email.
                    var newMails = emailContext.Email.Where (e => e.Status == EmailStatus.Opened);
                    var welcomeBody = GetWelcomeMessage();
                    newMails.ToList ().ForEach (i => {
                        emailClient.Send (i.From, i.To, "Welcome", welcomeBody.Replace ("{EmailID}", i.EmailId.ToString()));
                    });
                    //step 2: sending "Thank you" to all opened email.
                    var openedMails = emailContext.Email.Where (e => e.Status == EmailStatus.Opened);
                    openedMails.ToList ().ForEach (i => {
                        emailClient.Send (i.From, i.To, "Thank you", "body");
                    });
                    //step 3: sending Reminder to all 
                    var sentMails = emailContext.Email.Where (e => e.Status == EmailStatus.Sent && e.CreatedAt >= DateTime.UtcNow.AddDays (-3));
                    sentMails.ToList ().ForEach (i => {
                        emailClient.Send (i.From, i.To, "Gentel Reminder", "body");
                    });
                }
            }
        }

        //return welcome message from html template
        public static string GetWelcomeMessage () {
            var str = new StreamReader ("filePath");
            string mailText = str.ReadToEnd ();
            str.Close ();
            return mailText;
        }

        //Build Dependency injection and config from appsettings.json
        private static IServiceProvider BuildDi () {
            var config = new ConfigurationBuilder ()
                .SetBasePath (System.IO.Directory.GetCurrentDirectory ())
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)
                .Build ();
            var service = new ServiceCollection ()
                .AddSingleton (config)
                .AddSingleton<IEmailClient, EmailClient> ()
                .AddDbContext<EmailContext> (options =>
                    options.UseSqlServer (config.GetConnectionString ("EmailDB"))
                )
                .BuildServiceProvider ();
            ConnectionString = config.GetConnectionString ("EmailDB");
            interval = config.GetValue<double> ("App:Interval");
            var sendTime = config.GetValue<DateTime> ("App:SendTime");
            hours = sendTime.Hour;
            min = sendTime.Minute;
            return service;
        }
    }
}