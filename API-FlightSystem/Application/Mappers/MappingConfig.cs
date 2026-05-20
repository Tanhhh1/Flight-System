using Application.CQRS.Accounts.DTOs;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Flights.DTOs;
using Domain.Entities;
using Domain.Identity;
using Mapster;

namespace Application.Mappers
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, AccountDto>()
                .Map(dest => dest.UserId, src => src.Id)
                .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role.Name!).ToList());

            config.NewConfig<Flight, FlightDto>()
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange)
                .Map(dest => dest.Segments, src => src.FlightSegments)
                .Map(dest => dest.SeatPrices, src => src.FlightSeatPrices)
                .Map(dest => dest.Services, src => src.FlightServices);

            config.NewConfig<Flight, FlightListDto>()
                .Map(dest => dest.PlaneModel, src => src.Plane.PlaneModel)
                .Map(dest => dest.AirlineName, src => src.Plane.Airline.AirlineName)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City)
                .Map(dest => dest.FlightDuration, src => src.Route.FlightDuration)
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange)
                .Map(dest => dest.StopCount, src => src.FlightSegments.Count)
                .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<Flight, FlightDetailDto>()
                .Map(dest => dest.PlaneName, src => src.Plane.PlaneModel)
                .Map(dest => dest.AirlineName, src => src.Plane.Airline.AirlineName)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City)
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.Segments, src => src.FlightSegments)
                .Map(dest => dest.Services, src => src.FlightServices.Select(fs => fs.Service))
                .Map(dest => dest.FlightDuration, src => src.Route.FlightDuration)
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange);

            config.NewConfig<FlightSegment, FlightDetailSegmentDto>()
                .Map(dest => dest.StopOrder, src => src.SegmentOrder)
                .Map(dest => dest.DepartureTime, src => src.DepartureTime)
                .Map(dest => dest.ArrivalTime, src => src.ArrivalTime)
                .Map(dest => dest.FlightDuration, src => src.Route.FlightDuration)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City);

            config.NewConfig<Service, FlightDetailServiceDto>()
                .Map(dest => dest.ServiceName, src => src.ServiceName);

            config.NewConfig<Flight, FlightSearchDto>()
                .Map(dest => dest.AirlineName, src => src.Plane.Airline.AirlineName)
                .Map(dest => dest.PlaneName, src => src.Plane.PlaneModel)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City)
                .Map(dest => dest.FlightDuration, src => (int)(src.ArrivalTime - src.DepartureTime).TotalMinutes)
                .Map(dest => dest.StopCount, src => src.FlightSegments.Count - 1)
                .Map(dest => dest.SeatClasses, src => src.FlightSeatPrices)
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange);

            config.NewConfig<FlightSeatPrice, FlightSeatClassDto>()
                .Map(dest => dest.ClassId, src => src.ClassId)
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                .Map(dest => dest.Price, src => src.Price)
                .Map(dest => dest.AvailableSeats, src => src.AvailableSeats);

            config.NewConfig<Booking, BookingDto>()
                .Map(dest => dest.Fullname, src => src.User.Fullname)
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                .Map(dest => dest.TripType, src => src.TripType == 1 ? "Một chiều" : src.TripType == 2 ? "Khứ hồi" : src.TripType == 3 ? "Nhiều điểm đến" : "Không xác định");

            config.NewConfig<Booking, BookingListDto>()
                .Map(dest => dest.Fullname, src => src.User.Fullname)
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                .Map(dest => dest.TripType, src => src.TripType == 1 ? "Một chiều" : src.TripType == 2 ? "Khứ hồi" : src.TripType == 3 ? "Nhiều điểm đến" : "Không xác định");
        }
    }
}
