using Developer.Api.Domain;
using Developer.Api.Repositories;
using Developer.Api.Requests;
using EnsureThat;
using FluentValidation;
using MediatR;

namespace Developer.Api.Handlers
{
    public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
    {
        public UpdateProjectRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty().Length(1, 32);

            RuleFor(request => request.Description).NotEmpty().Length(1, 1024);
        }
    }

    public class UpdateProjectRequest : IRequest<UpdateProjectResponse>
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }

    public class UpdateProjectResponse
    {
        public Project? Project { get; set; }

        public bool Status { get; set; }
    }

    public class UpdateProjectHandler : IRequestHandler<UpdateProjectRequest, UpdateProjectResponse>
    {
        private IProjectRepository Repository { get; }

        public UpdateProjectHandler(IProjectRepository repository)
        {
            Repository = EnsureArg.IsNotNull(repository);
        }

        public async Task<UpdateProjectResponse> Handle(UpdateProjectRequest request, CancellationToken token)
        {
            var result = new UpdateProjectResponse();

            result.Status = await Repository.UpdateProject(request.Id, request.Name, request.Description);

            var counter = await Repository.GetProject(request.Id);

            if (counter != null)
            {
                result.Project = new Project
                {
                    Name = request.Name,
                    Description = counter.Description
                };
            }

            return result;
        }
    }
}