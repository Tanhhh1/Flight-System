using API_FlightSystem.Controllers.Common;
using Application.Common;
using Application.CQRS.Reviews.Commands.Send;
using Application.CQRS.Reviews.DTOs;
using Application.CQRS.Reviews.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1.Client
{
    public class ReviewController : ApiController
    {
        private readonly IMediator _mediator;
        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllReviewQuery query)
        {
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendReview([FromBody] SendReviewCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
