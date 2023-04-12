using Developer.Api.Domain;
using Developer.Api.Requests;
using Developer.Api.Responses;
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
        public async Task<ListProjectResponse> Get(ListProjectRequest request)
        {
            var result = new ListProjectResponse();

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.GetProjectsRequest());

                if (response != null)
                {
                    result.Projects = response.Projects;

                    result.Status = response.Status;
                }
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ViewProjectResponse> Get(ViewProjectRequest request)
        {
            var result = new ViewProjectResponse();

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.GetProjectRequest { Id = request.Id });

                if (response != null)
                {
                    result.Project = response.Project;

                    result.Status = response.Status;
                }
            }

            return result;
        }

        [HttpPost("")]
        public async Task<CreateProjectResponse> Post([FromBody] CreateProjectRequest request)
        {
            var result = new CreateProjectResponse();

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.CreateProjectRequest { Name = request.Name, Description = request.Description });

                if (response != null)
                {
                    result.Project = response.Project;

                    result.Status = response.Status;
                }
            }

            return result;
        }

        [HttpPut("")]
        public async Task<UpdateProjectResponse> Put([FromBody] UpdateProjectRequest request)
        {
            var result = new UpdateProjectResponse();

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.UpdateProjectRequest { Id = request.Id, Name = request.Name, Description = request.Description });

                if (response != null)
                {
                    result.Project = response.Project;

                    result.Status = response.Status;
                }
            }

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<DeleteProjectResponse> Delete(DeleteProjectRequest request)
        {
            var result = new DeleteProjectResponse();

            if (ModelState.IsValid)
            {
                var response = await Mediator.Send(new Handlers.DeleteProjectRequest { Id = request.Id });

                if (response != null)
                {
                    result.Status = response.Status;
                }
            }

            return result;
        }
    }
}