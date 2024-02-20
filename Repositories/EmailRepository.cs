using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ServerlessLogin.Helpers;
using ServerlessLogin.Interfaces;
using ServerlessLogin.Models;
using System.Globalization;
using System.Reflection;

namespace ServerlessLogin.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly MailSettings _mailSettings;

        public EmailRepository(IOptions<MailSettings> mailSettingOptions)
        {

            _mailSettings = mailSettingOptions.Value;

        }

        public async Task<bool> SendValidationCodeEmail(User user, string validationCode)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.SenderEmail));
                    emailMessage.To.Add(MailboxAddress.Parse(user.Email));
                    emailMessage.Subject = "API Email Verification";

                    var reader = new ResourcesHelper();
                    string emailTemplateText = reader.ReadResource("Templates.EmailCodeActivation.html");


                    // Replace HTML Text with values
                    DateTime now = DateTime.UtcNow;
                    CultureInfo culture = new("en-US");

                    emailTemplateText = emailTemplateText.Replace("{{code}}", validationCode);
                    emailTemplateText = emailTemplateText.Replace("{{month}}", now.ToString("MMM", culture));
                    emailTemplateText = emailTemplateText.Replace("{{year}}", now.Year.ToString());

                    BodyBuilder bodyBuilder = new()
                    {
                        HtmlBody = emailTemplateText,
                    };

                    emailMessage.Body = bodyBuilder.ToMessageBody();

                    using (SmtpClient mailClient = new())
                    {
                        await mailClient.ConnectAsync(
                            _mailSettings.Server,
                            Int32.Parse(_mailSettings.Port),
                            SecureSocketOptions.StartTls
                        );
                        await mailClient.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }

                }
                return true;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return false;
            }

        }
    }
}
