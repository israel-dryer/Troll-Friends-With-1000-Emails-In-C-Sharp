using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Troll_Friends_With_1000_Emails_In_C_Sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            // program settings
            int mailNumberTarget = 1000;
            int delayInMilleseconds = 10000;
            string responseText = DownloadBookText("https://www.gutenberg.org/files/2600/2600-0.txt");
            
            // email credentials
            string username = "username@gmail.com";
            string password = "password";

            // parse book text and mail
            var bookTextChunks = ParseBookChunks(responseText, mailNumberTarget);
            var running_count = 0;
            foreach (var chunk in bookTextChunks)
            {
                running_count++;
                Console.WriteLine($"Sending message # {running_count} of 1000");
                SendEmailMessage(chunk, running_count, username, password);
                Thread.Sleep(delayInMilleseconds);
            }
        }

        public static void SendEmailMessage(string message, int running_count, string username, string password)
        {
            var mailMessage = new MimeMessage();
            
            // add the sender and recipient here....
            mailMessage.From.Add(new MailboxAddress("SenderName", "Sender@gmail.com"));
            mailMessage.To.Add(new MailboxAddress("RecipientName", "recipient@gmail.com"));
            mailMessage.Subject = $"Trolling you with a book: {running_count} of 1000!";
            mailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(username, password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }

        public static string DownloadBookText(string url)
        {
            var web = new WebClient();
            var responseText = web.DownloadString(url);
            return responseText;
        }

        public static List<string> ParseBookChunks(string responseText, int mailNumberTarget)
        {
            var textChunkList = new List<string>();
            var textArray = responseText.Split(" ");
            var chunkSize = (int)Math.Ceiling((double)textArray.Length / (double)mailNumberTarget);
            var numChunks = textArray.Length / chunkSize;
            var lastChunkSize = textArray.Length % chunkSize;

            // concatenate chunks into `chunkSize` strings and add to `textChunkList`
            for (int i = 0; i < numChunks; i++)
            {
                var chunkString = String.Join(" ", textArray.Skip(i * chunkSize).Take(chunkSize).ToArray());
                var cleanString = Regex.Replace(chunkString, @"\n|\r", "");
                textChunkList.Add(cleanString);
            }
            if (lastChunkSize > 0)
            {
                var chunkString = String.Join(" ", textArray.Skip(numChunks * chunkSize).Take(lastChunkSize).ToArray());
                var cleanString = Regex.Replace(chunkString, @"\n|\r", "");
                textChunkList.Add(cleanString);
            }
            return textChunkList;
        }
    }
}
