using Application.Common;
using Application.CQRS.Profile.DTOs;
using MediatR;

namespace Application.CQRS.Profile.Queries.GetById
{
    public class GetByUserIdQuery : IRequest<ApiResult<UserProfileDto>>
    {
    }
}
