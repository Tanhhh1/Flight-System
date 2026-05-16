using Application.CQRS.Accounts.DTOs;
using Application.CQRS.Flights.DTOs;
using Application.CQRS.Planes.DTOs;
using Application.CQRS.Routes.DTOs;
using Domain.Entities;
using Domain.Identity;
using Mapster;

namespace Application.Mappers
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Route, RouteDto>()
                .Map(dest => dest.OriginAirportCode, src => src.OriginAirport.AirportCode)
                .Map(dest => dest.DestinationAirportCode, src => src.DestinationAirport.AirportCode);

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
                .Map(dest => dest.OriginAirport, src => src.Route.OriginAirport.AirportName)
                .Map(dest => dest.DestinationAirport, src => src.Route.DestinationAirport.AirportName)
                .Map(dest => dest.FlightDuration, src => src.Route.FlightDuration)
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange)
                .Map(dest => dest.SeatPrices, src => src.FlightSeatPrices);

            config.NewConfig<Flight, FlightSearchDto>()
                .Map(dest => dest.FlightDuration, src => src.Route.FlightDuration)
                .Map(dest => dest.PlaneModel, src => src.Plane.PlaneModel)
                .Map(dest => dest.AirlineId, src => src.Plane.AirlineId)
                .Map(dest => dest.AirlineName, src => src.Plane.Airline.AirlineName)
                .Map(dest => dest.OriginAirportId, src => src.Route.OriginAirportId)
                .Map(dest => dest.OriginAirportCode, src => src.Route.OriginAirport.AirportCode)
                .Map(dest => dest.OriginCity, src => src.Route.OriginAirport.City)
                .Map(dest => dest.DestinationAirportId, src => src.Route.DestinationAirportId)
                .Map(dest => dest.DestinationAirportCode, src => src.Route.DestinationAirport.AirportCode)
                .Map(dest => dest.DestinationCity, src => src.Route.DestinationAirport.City)
                .Map(dest => dest.IsRefund, src => src.Policy.IsRefund)
                .Map(dest => dest.IsChange, src => src.Policy.IsChange)
                .Map(dest => dest.StopCount, src => src.FlightSegments.Count)
                .Map(dest => dest.Services, src => src.FlightServices);

        }
    }
}
