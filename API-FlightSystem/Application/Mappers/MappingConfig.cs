using Application.CQRS.Accounts.DTOs;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Flights.DTOs;
using Application.CQRS.Planes.DTOs;
using Application.CQRS.Reviews.DTOs;
using Application.CQRS.Routes.DTOs;
using Application.CQRS.Support.DTOs;
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

            config.NewConfig<Plane, PlaneDto>()
                .Map(dest => dest.AirlineName, src => src.Airline.AirlineName);

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
                .Map(dest => dest.IsChange, src => src.Policy.IsChange)
                .Map(dest => dest.SeatPrices, src => src.FlightSeatPrices);

            config.NewConfig<FlightSegment, FlightDetailSegmentDto>()
                .Map(dest => dest.StopOrder, src => src.SegmentOrder)
                .Map(dest => dest.DepartureTime, src => src.DepartureTime)
                .Map(dest => dest.ArrivalTime, src => src.ArrivalTime)
                .Map(dest => dest.FlightDuration, src => src.Route.FlightDuration)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City);

            config.NewConfig<FlightSeatPrice, FlightDetailSeatPriceDto>()
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName);

            config.NewConfig<Flight, FlightSearchDto>()
                .Map(dest => dest.AirlineName, src => src.Plane.Airline.AirlineName)
                .Map(dest => dest.PlaneName, src => src.Plane.PlaneModel)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City)
                .Map(dest => dest.FlightDuration, src => (int)(src.ArrivalTime - src.DepartureTime).TotalMinutes)
                .Map(dest => dest.StopCount, src => src.FlightSegments.Count - 1)
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange)
                .Map(dest => dest.SeatClasses, src => src.FlightSeatPrices)
                .Map(dest => dest.Segments, src => src.FlightSegments.OrderBy(s => s.SegmentOrder))
                .Map(dest => dest.Services, src => src.FlightServices.Select(fs => fs.Service));

            config.NewConfig<FlightSeatPrice, FlightSeatClassDto>()
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                .Map(dest => dest.AvailableSeats, src => src.AvailableSeats);

            config.NewConfig<Booking, BookingDto>()
                .Map(dest => dest.Fullname, src => src.User.Fullname)
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                .Map(dest => dest.TripType, src => src.TripType.ToString());

            config.NewConfig<Booking, BookingByIdDto>()
                .Map(dest => dest.Fullname, src => src.User.Fullname)
                .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                .Map(dest => dest.TripType, src => src.TripType.ToString())
                .Map(dest => dest.Flights, src => src.BookingDetails
                    .GroupBy(bd => bd.BookingFlightId)
                    .Select(g => new BookingFlightDto
                    {
                        FlightId = g.Key,
                        DepartureTime = g.First().Flight.DepartureTime,
                        ArrivalTime = g.First().Flight.ArrivalTime,
                        FlightStatus = g.First().Flight.Status.ToString(),
                        AirlineName = g.First().Flight.Plane.Airline.AirlineName,
                        PlaneModel = g.First().Flight.Plane.PlaneModel,
                        OriginAirport = g.First().Flight.Route.OriginAirport.AirportCode,
                        OriginAirportName = g.First().Flight.Route.OriginAirport.AirportName,
                        DestinationAirport = g.First().Flight.Route.DestinationAirport.AirportCode,
                        DestinationAirportName = g.First().Flight.Route.DestinationAirport.AirportName,
                        FlightDuration = g.First().Flight.Route.FlightDuration,
                        Segments = g.First().Flight.FlightSegments
                        .OrderBy(s => s.SegmentOrder)
                        .Select(s => new BookingSegmentDto
                        {
                            SegmentOrder = s.SegmentOrder,
                            OriginAirport = s.Route.OriginAirport.AirportCode,
                            OriginAirportName = s.Route.OriginAirport.AirportName,
                            DestinationAirport = s.Route.DestinationAirport.AirportCode,
                            DestinationAirportName = s.Route.DestinationAirport.AirportName,
                            DepartureTime = s.DepartureTime,
                            ArrivalTime = s.ArrivalTime,
                            FlightDuration = s.Route.FlightDuration,
                        }).ToList(),
                        Passengers = g.Select(bd => new PassengerDetailDto
                        {
                            TypeId = bd.Passenger.TypeId,
                            FullName = bd.Passenger.FullName,
                            Gender = bd.Passenger.Gender,
                            Birthday = bd.Passenger.Birthday,
                            Country = bd.Passenger.Country,
                            UnitPrice = bd.UnitPrice
                        }).ToList()
                    }).ToList());

            config.NewConfig<Booking, BookingListDto>()
                 .Map(dest => dest.Fullname, src => src.User.Fullname)
                 .Map(dest => dest.ClassName, src => src.SeatClass.ClassName)
                 .Map(dest => dest.TripType, src => src.TripType.ToString())
                 .Map(dest => dest.OriginAirport, src => src.BookingDetails
                     .Select(bd => bd.Flight.Route.OriginAirport.AirportCode)
                     .FirstOrDefault() ?? string.Empty)
                 .Map(dest => dest.DestinationAirport, src => src.BookingDetails
                     .Select(bd => bd.Flight.Route.DestinationAirport.AirportCode)
                     .FirstOrDefault() ?? string.Empty);

            config.NewConfig<Review, ReviewDto>()
                .Map(dest => dest.UserName, src => src.User.UserName)
                .Map(dest => dest.UserEmail, src => src.User.Email);

            config.NewConfig<Route, RouteDto>()
                .Map(dest => dest.OriginAirportCode, src => src.OriginAirport.AirportCode)
                .Map(dest => dest.DestinationAirportCode, src => src.DestinationAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.OriginAirport.City)
                .Map(dest => dest.DestinationCity, src => src.DestinationAirport.City);

            config.NewConfig<Route, DataRouteDto>()
                .Map(dest => dest.OriginAirportCode, src => src.OriginAirport.AirportCode)
                .Map(dest => dest.OriginAirportName, src => src.OriginAirport.AirportName)
                .Map(dest => dest.DestinationAirportCode, src => src.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationAirportName, src => src.DestinationAirport.AirportName);

            config.NewConfig<Plane, DataPlaneDto>()
                .Map(dest => dest.AirlineName, src => src.Airline.AirlineName);

            config.NewConfig<SupportRequest, SupportRequestDto>()
                .Map(dest => dest.BookingCode, src => src.Booking.BookingCode)
                .Map(dest => dest.Fullname, src => src.Booking.User.Fullname)
                .Map(dest => dest.RequestType, src => src.RequestType.ToString())
                .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<SupportRequest, SupportRequestDetailDto>()
                .Map(dest => dest.BookingCode, src => src.Booking.BookingCode)
                .Map(dest => dest.CustomerName, src => src.Booking.User.Fullname)
                .Map(dest => dest.RequestType, src => src.RequestType.ToString())
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.NewFlight, src => src.NewFlight);

            config.NewConfig<Flight, NewFlightInfoDto>()
                .Map(dest => dest.OriginAirport, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginAirportName, src => src.Route.OriginAirport.AirportName)
                .Map(dest => dest.DestinationAirport, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationAirportName, src => src.Route.DestinationAirport.AirportName);
        }
    }
}
