using Developer.Api.Domain;
using Developer.Api.Repositories;
using Developer.Api.Requests;
using EnsureThat;
using FluentValidation;
using MediatR;

namespace Developer.Api.Handlers
{
    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty().Length(1, 32);

            RuleFor(request => request.Description).NotEmpty().Length(1, 1024);
        }
    }

    public class CreateProjectRequest : IRequest<CreateProjectResponse>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }
    }

    public class CreateProjectResponse
    {
        public Project? Project { get; set; }

        public bool Status { get; set; }
    }

    public class CreateProjectHandler : IRequestHandler<CreateProjectRequest, CreateProjectResponse>
    {
        private IProjectRepository Repository { get; }

        public CreateProjectHandler(IProjectRepository repository)
        {
            Repository = EnsureArg.IsNotNull(repository);
        }

        public async Task<CreateProjectResponse> Handle(CreateProjectRequest request, CancellationToken token)
        {
            var result = new CreateProjectResponse();

            Guid id = Guid.NewGuid();

            result.Status = await Repository.CreateProject(id, request.Name, request.Description);

            var entity = await Repository.GetProject(id);

            if (entity != null)
            {
                result.Project = new Project
                {
                    Id = id,
                    Name = entity.Name,
                    Description = entity.Description
                };
            }

            return result;
        }
    }
}