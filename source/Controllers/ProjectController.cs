using Developer.Api.Domain;
using Developer.Api.Requests;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Developer.Api.Controllers
{
    [Route("projects")]
    public class ProjectController : ControllerBase
    {
        private IMediator Mediator { get; }

        public ProjectController(IMediator mediator)
        {
            Mediator = EnsureArg.IsNotNull(mediator);
        }

        [HttpGet("")]
        public Task<List<Project>> Get()
        {
            var result = new List<Project>();

            return Task.FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<Project?> Get(ViewProjectRequest request)
        {
            Project? result = null;

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.GetProjectRequest { Id = request.Id });

                if (response != null && response.Status)
                {
                    result = response.Project;
                }
            }

            return result;
        }

        [HttpPost("")]
        public async Task<Project?> Post([FromBody] CreateProjectRequest request)
        {
            Project? result = null;

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.CreateProjectRequest { Name = request.Name, Description = request.Description });

                if (response != null && response.Status)
                {
                    result = response.Project;
                }
            }

            return result;
        }

        [HttpPut("")]
        public async Task<Project?> Put([FromBody] UpdateProjectRequest request)
        {
            Project? result = null;

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.UpdateProjectRequest { Id = request.Id, Name = request.Name, Description = request.Description });

                if (response != null && response.Status)
                {
                    result = response.Project;
                }
            }

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(DeleteProjectRequest request)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.DeleteProjectRequest { Id = request.Id });

                if (response != null)
                {
                    result = response.Status;
                }
            }

            return result;
        }
    }
}