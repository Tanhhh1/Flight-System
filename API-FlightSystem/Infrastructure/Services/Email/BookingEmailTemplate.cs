using System.Text;
using Application.Services;

namespace Infrastructure.Services.Email
{
    public static class BookingEmailTemplate
    {
        public static string Build(BookingConfirmationEmailDto dto, string supportEmail)
        {
            var flightsHtml = new StringBuilder();
            for (int i = 0; i < dto.Flights.Count; i++)
            {
                var f = dto.Flights[i];
                var flightNo = dto.Flights.Count > 1 ? $"Chặng {i + 1}" : "Chuyến bay";
                var duration = (int)(f.ArrivalTime - f.DepartureTime).TotalMinutes;
                var durationStr = $"{duration / 60}h{duration % 60:D2}m";

                var passengersHtml = new StringBuilder();
                foreach (var p in f.Passengers)
                {
                    passengersHtml.Append($"""
                        <tr style="border-top: 1px solid #f0f0f0;">
                            <td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">{p.FullName} ({p.Gender})</td>
                            <td style="padding: 10px 16px; color: #111111;">{p.UnitPrice:N0} VND</td>
                        </tr>
                        """);
                }

                flightsHtml.Append($"""
                    <div style="border: 1px solid #e0e0e0; border-radius: 8px; margin-bottom: 16px; overflow: hidden;">
                        <div style="background: #f0f5ff; padding: 10px 16px; font-size: 13px; font-weight: bold; color: #1a3c5e;">
                            <i>{flightNo}</i>: {f.OriginAirport} &rarr; {f.DestinationAirport}
                        </div>
                        <div>
                            <table style="width: 100%; border-collapse: collapse; font-size: 14px;">
                                <tr>
                                    <td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Hãng bay</td>
                                    <td style="padding: 10px 16px; color: #111111;">{f.AirlineName} &middot; {f.PlaneModel}</td>
                                </tr>
                                <tr style="border-top: 1px solid #e0e0e0;">
                                    <td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Khởi hành</td>
                                    <td style="padding: 10px 16px; color: #111111;">{f.DepartureTime:HH:mm dd/MM/yyyy} &middot; {f.OriginAirportName}</td>
                                </tr>
                                <tr style="border-top: 1px solid #e0e0e0;">
                                    <td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Đến nơi</td>
                                    <td style="padding: 10px 16px; color: #111111;">{f.ArrivalTime:HH:mm dd/MM/yyyy} &middot; {f.DestinationAirportName}</td>
                                </tr>
                                <tr style="border-top: 1px solid #e0e0e0;">
                                    <td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Thời gian bay</td>
                                    <td style="padding: 10px 16px; color: #111111;">{durationStr}</td>
                                </tr>
                                {passengersHtml}
                            </table>
                        </div>
                    </div>
                    """);
            }

            return $"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8" />
                </head>
                <body style="margin:0; padding:0; background:#f4f4f4; font-family:Arial,sans-serif;">
                    <table width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="center" style="padding:32px 16px;">
                                <table width="580" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="background: #1a3c5e; padding: 24px 32px;">
                                            <p style="margin: 0; font-size: 20px; font-weight: bold; color: #ffffff;">Đặt vé thành công</p>
                                            <p style="margin: 4px 0 0; font-size: 13px; color: #a0bcd8;">Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi</p>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="padding: 24px 32px; background: #ffffff;">
                                            <p style="margin: 0 0 16px; font-size: 14px; color: #555555;">Xin chào <strong>{dto.CustomerName}</strong>,</p>
                                            <p style="margin: 0 0 24px; font-size: 14px; color: #555555; line-height: 1.6;">Booking của bạn đã được xác nhận. Vui lòng kiểm tra thông tin bên dưới.</p>

                                            <p style="margin: 0 0 8px; font-size: 13px; font-weight: bold; color: #1a3c5e; text-transform: uppercase;">Thông tin đặt vé</p>
                                            <table style="width: 100%; border: 1px solid #e0e0e0; border-collapse: collapse; font-size: 14px; margin-bottom: 24px;">
                                                <tr><td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Mã booking</td><td style="padding: 10px 16px; color: #111111;"><strong>{dto.BookingCode}</strong></td></tr>
                                                <tr style="border-top: 1px solid #e0e0e0;"><td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Loại chuyến</td><td style="padding: 10px 16px; color: #111111;">{dto.TripType}</td></tr>
                                                <tr style="border-top: 1px solid #e0e0e0;"><td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Ngày đặt</td><td style="padding: 10px 16px; color: #111111;">{dto.BookingDate:dd/MM/yyyy HH:mm}</td></tr>
                                                <tr style="border-top: 1px solid #e0e0e0;"><td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Phương thức</td><td style="padding: 10px 16px; color: #111111;">{dto.PaymentMethod}</td></tr>
                                                <tr style="border-top: 1px solid #e0e0e0;"><td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Tổng tiền</td><td style="padding: 10px 16px; color: #111111;"><strong>{dto.TotalPrice:N0} VND</strong></td></tr>
                                                <tr style="border-top: 1px solid #e0e0e0;"><td style="padding: 10px 16px; color: #888888; width: 45%; background: #f9f9f9;">Trạng thái</td><td style="padding: 10px 16px; color: #111111;"><span style="background: #e6f4ea; color: #1e6b3a; font-size: 12px; padding: 3px 10px; border-radius: 12px; font-weight: bold;">Đã thanh toán</span></td></tr>
                                            </table>

                                            <p style="margin: 0 0 8px; font-size: 13px; font-weight: bold; color: #1a3c5e; text-transform: uppercase;">Chi tiết chuyến bay</p>
                                            {flightsHtml}

                                            <p style="margin: 16px 0 0; font-size: 13px; color: #888888; border-top: 1px solid #e0e0e0; padding-top: 16px; line-height: 1.6;">
                                                Nếu có thắc mắc, vui lòng liên hệ <a href="mailto:{supportEmail}" style="color: #185fa5; text-decoration: none;">{supportEmail}</a>
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """;
        }
    }
}