using Application.CQRS.Accounts.DTOs;
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
        }
    }
}
