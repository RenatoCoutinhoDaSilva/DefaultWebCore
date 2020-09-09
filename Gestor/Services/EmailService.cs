using Gestor.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gestor.Services
{
    public class EmailService : IEmailService
    {
        private SmtpClient smtpClient;
        private List<Attachment> Attachments;

        public string Body { get; set; }
        public bool IsHTML { get; set; } = false;
        public bool EnableDebug { get; set; } = true;
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }

        public EmailService(IConfiguration config) {
            var host = config.GetValue<string>("EmailService:Host");
            var port_str = config.GetValue<string>("EmailService:Port");
            var ssl_str = config.GetValue<string>("EmailService:SSL");
            var user = config.GetValue<string>("EmailService:User");
            var password = config.GetValue<string>("EmailService:Password");

            SenderEmail = config.GetValue<string>("EmailService:SenderEmail");
            SenderName = config.GetValue<string>("EmailService:SenderName");
            Attachments = new List<Attachment>();

            var ssl = ssl_str == "true";
            var port = 0;
            int.TryParse(port_str, out port);

            smtpClient = new SmtpClient {
                Host = host,
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(user, password)
            };
        }

        public void AddBodyProperty(string key, string value) {
            VerifyBody();
            Body = Body.Replace(key, value);
        }

        public void AddAttachment(string path) {
            VerifyBody();
            Attachments.Add(new Attachment(path));
        }


        public async Task SendToList(List<string> destinyList, string subject) {
            VerifyBody();

            foreach (var destiny in destinyList) {
                if (IsValidEmail(destiny)) {
                    await Send(destiny, subject);
                }
            }
        }

        public async Task Send(string destiny, string subject) {
            VerifyBody();

            var mail = new MailMessage();
            if (IsValidEmail(destiny)) {
                try {
                    mail.To.Add(destiny);
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.Subject = subject;
                    mail.Body = Body;
                    mail.IsBodyHtml = IsHTML;

                    if (Attachments.Count > 0) {
                        foreach (var file in Attachments)
                            mail.Attachments.Add(file);
                    }

                    await smtpClient.SendMailAsync(mail);
                } catch (Exception e) {
                    if (EnableDebug) {
                        throw new Exception($"Houve um erro no envio de e-mail com a mensagem: {e.Message}");
                    }
                }
            } else throw new Exception("Destinatário inválido.");
        }

        private void VerifyBody() {
            if (string.IsNullOrEmpty(Body)) {
                throw new Exception("Nenhum corpo de e-mail especificado. Use o método SetBody para especificar um.");
            }
        }
        private bool IsValidEmail(string strIn) {
            if (String.IsNullOrEmpty(strIn))
                return false;

            try {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            } catch (RegexMatchTimeoutException) {
                return false;
            }
        }
    }
}
