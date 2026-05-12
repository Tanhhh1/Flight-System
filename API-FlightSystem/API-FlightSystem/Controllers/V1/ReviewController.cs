using API_FlightBooking.Controllers.Common;
using Application.Common;
using Application.CQRS.Reviews.Commands.Delete;
using Application.CQRS.Reviews.Commands.Send;
using Application.CQRS.Reviews.DTOs;
using Application.CQRS.Reviews.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API_FlightSystem.Controllers.V1
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
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendReview([FromBody] SendReviewCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<PageList<ReviewDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteReviewCommand { ReviewId = id });
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
