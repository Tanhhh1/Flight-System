namespace Application.Services
{
    public record BookingConfirmationEmailDto(
        string ToEmail,
        string CustomerName,
        string BookingCode,
        string TripType,
        string PaymentMethod,
        decimal TotalPrice,
        DateTime BookingDate,
        List<FlightEmailDto> Flights
    );

    public record FlightEmailDto(
        string OriginAirport,
        string OriginAirportName,
        string DestinationAirport,
        string DestinationAirportName,
        DateTime DepartureTime,
        DateTime ArrivalTime,
        string AirlineName,
        string PlaneModel,
        List<PassengerEmailDto> Passengers
    );

    public record PassengerEmailDto(
        string FullName,
        string Gender,
        decimal UnitPrice
    );

    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(BookingConfirmationEmailDto dto);
    }
}