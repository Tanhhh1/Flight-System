using Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Services
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
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Email:From"]!));
            message.To.Add(MailboxAddress.Parse(dto.ToEmail));
            message.Subject = $"Xác nhận đặt vé thành công - {dto.BookingCode}";

            var html = BuildEmailBody(dto);
            var inlined = PreMailer.Net.PreMailer.MoveCssInline(html);

            message.Body = new TextPart("html") { Text = inlined.Html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["Email:Host"]!,
                int.Parse(_config["Email:Port"]!),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                _config["Email:Username"]!,
                _config["Email:Password"]!);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        private string BuildEmailBody(BookingConfirmationEmailDto dto)
        {
            var supportEmail = _config["Email:From"];

            var style = "<style>" +
                ".header { background:#1a3c5e; padding:24px 32px; }" +
                ".header-title { margin:0; font-size:20px; font-weight:bold; color:#ffffff; }" +
                ".header-sub { margin:4px 0 0; font-size:13px; color:#a0bcd8; }" +
                ".body { padding:24px 32px; background:#ffffff; }" +
                ".greeting { margin:0 0 16px; font-size:14px; color:#555555; }" +
                ".intro { margin:0 0 24px; font-size:14px; color:#555555; line-height:1.6; }" +
                ".info-table { width:100%; border:1px solid #e0e0e0; border-collapse:collapse; font-size:14px; }" +
                ".label { padding:10px 16px; color:#888888; width:45%; background:#f9f9f9; }" +
                ".value { padding:10px 16px; color:#111111; }" +
                ".row-border { border-top:1px solid #e0e0e0; }" +
                ".badge { background:#e6f4ea; color:#1e6b3a; font-size:12px; padding:3px 10px; border-radius:12px; font-weight:bold; }" +
                ".footer { margin:16px 0 0; font-size:13px; color:#888888; border-top:1px solid #e0e0e0; padding-top:16px; line-height:1.6; }" +
                ".footer a { color:#185fa5; text-decoration:none; }" +
                "</style>";

            return "<!DOCTYPE html><html><head>" + style + "</head>" +
                "<body style='margin:0; padding:0; background:#f4f4f4; font-family:Arial,sans-serif;'>" +
                "<table width='100%' cellpadding='0' cellspacing='0'>" +
                "<tr><td align='center' style='padding:32px 16px;'>" +
                "<table width='520' cellpadding='0' cellspacing='0'>" +

                // Header
                "<tr><td class='header'>" +
                "<p class='header-title'>Đặt vé thành công</p>" +
                "<p class='header-sub'>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi</p>" +
                "</td></tr>" +

                // Body
                "<tr><td class='body'>" +
                "<p class='greeting'>Xin chào <strong>" + dto.CustomerName + "</strong>,</p>" +
                "<p class='intro'>Booking của bạn đã được xác nhận. Vui lòng kiểm tra thông tin bên dưới.</p>" +

                // Info table
                "<table class='info-table'>" +
                "<tr><td class='label'>Mã booking</td><td class='value'><strong>" + dto.BookingCode + "</strong></td></tr>" +
                "<tr class='row-border'><td class='label'>Loại chuyến</td><td class='value'>" + dto.TripType + "</td></tr>" +
                "<tr class='row-border'><td class='label'>Ngày đặt</td><td class='value'>" + dto.BookingDate.ToString("dd/MM/yyyy HH:mm") + "</td></tr>" +
                "<tr class='row-border'><td class='label'>Phương thức</td><td class='value'>" + dto.PaymentMethod + "</td></tr>" +
                "<tr class='row-border'><td class='label'>Tổng tiền</td><td class='value'><strong>" + dto.TotalPrice.ToString("N0") + " VND</strong></td></tr>" +
                "<tr class='row-border'><td class='label'>Trạng thái</td><td class='value'><span class='badge'>Đã thanh toán</span></td></tr>" +
                "</table>" +

                "<p class='footer'>Nếu có thắc mắc, vui lòng liên hệ <a href='mailto:" + supportEmail + "'>" + supportEmail + "</a></p>" +
                "</td></tr>" +

                "</table></td></tr></table>" +
                "</body></html>";
        }
    }
}