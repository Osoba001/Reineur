using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Infrastucture.PermissionAuthorizations;

namespace Reineur.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Permission.Administrator)]
    public class AdminsController(IMediator mediator, ILogger<CustomControllerBase> logger) : CustomControllerBase(mediator, logger)
    {

    }
}
