using Microsoft.AspNetCore.Mvc;
using Reineur.Share;
using System.Net;
using System.Security.Claims;

namespace Reineur.API.Controllers.Base
{
    public class CustomControllerBase(IMediator mediator, ILogger<CustomControllerBase> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<CustomControllerBase> _logger = logger;

        public async Task<IActionResult> SendRequestAsync<TRequest>(TRequest request) where TRequest : Request
        {
            var result = request.Validate();
            if (!result.IsSuccess) return BadRequest(result.ReasonPhrase);
            result = await _mediator.SendAsync(request);
            if (result.Information != null)
            {
                //_logger.LogWarning(result.Information);
            }
            if (!result.IsSuccess)
            {
                if (result.ErrorType == ResponseType.NotFound)
                    return NotFound(result.ReasonPhrase);
                if (result.ServerException != null)
                {
                    _logger.LogError("Something went wrong. Unhandle exception: \n\n{message} ", result.ServerException);
                    return Problem(result.ReasonPhrase, statusCode: (int)HttpStatusCode.InternalServerError);
                }
                if (result.ErrorType == ResponseType.Unauthorized)
                    return Unauthorized(result.ReasonPhrase);
                return BadRequest(result.ReasonPhrase);
            }
            if (result.PayLoad is not null)
                return Ok(result.PayLoad);
            return NoContent();
        }

        internal Guid UserId
        {
            get
            {
                if (User is null) return default;
                var result = User.FindFirst(ClaimTypes.NameIdentifier);
                if (result is not null)
                    return Guid.Parse(result.Value);
                return default;
            }
        }
    }
}
