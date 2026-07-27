using Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendBookingConfirmationAsync(BookingConfirmationEmailDto dto)
        {
            try
            {
                var fromEmail = _config["Email:From"]!;
                var htmlBody = BookingEmailTemplate.Build(dto, fromEmail);

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(fromEmail));
                message.To.Add(MailboxAddress.Parse(dto.ToEmail));
                message.Subject = $"Xác nhận đặt vé thành công - {dto.BookingCode}";
                message.Body = new TextPart("html") { Text = htmlBody };

                using var smtp = new SmtpClient();
                smtp.Timeout = 10000;

                await smtp.ConnectAsync(_config["Email:Host"]!, int.Parse(_config["Email:Port"]!), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["Email:Username"]!, _config["Email:Password"]!);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Service Error]: {ex.Message}");
            }
        }
    }
}