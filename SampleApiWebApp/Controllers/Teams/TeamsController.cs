using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RequestManagement;
using SampleApiWebApp.Constants;
using SampleApiWebApp.Controllers.Teams.GetOne;
using SampleApiWebApp.Controllers.Teams.Post;
using SampleApiWebApp.Controllers.Teams.Put;

namespace SampleApiWebApp.Controllers.Teams
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TeamsController : Controller
    {
        public TeamsController(IMediator mediator)
        {
            this.Mediator = mediator;
        }

        private IMediator Mediator { get; }

        [HttpPost]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> Post(
            [FromBody]PostTeamCommand request,
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await this.Mediator.Send(request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet]
        [Route("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200, Type = typeof(Team))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOne([FromRoute]long id, CancellationToken cancellationToken = default)
        {
            var request = new GetTeamRequest { Id = id };
            var result = await this.Mediator.Send(request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut]
        [Route("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(
            [FromRoute]long id,
            [FromBody]PutTeamCommand request,
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.Id = id;

            var result = await this.Mediator.Send(request, cancellationToken);

            return result.ToActionResult();
        }
    }
}
