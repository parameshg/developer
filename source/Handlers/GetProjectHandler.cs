using Developer.Api.Domain;
using Developer.Api.Repositories;
using EnsureThat;
using FluentValidation;
using MediatR;

namespace Developer.Api.Handlers
{
    public class GetProjectRequestValidator : AbstractValidator<GetProjectRequest>
    {
        public GetProjectRequestValidator()
        {
            RuleFor(request => request.Id).NotEmpty();
        }
    }

    public class GetProjectRequest : IRequest<GetProjectResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetProjectResponse
    {
        public Project? Project { get; set; }

        public bool Status { get; set; }
    }

    public class GetProjectHandler : IRequestHandler<GetProjectRequest, GetProjectResponse>
    {
        private IProjectRepository Repository { get; }

        public GetProjectHandler(IProjectRepository repository)
        {
            Repository = EnsureArg.IsNotNull(repository);
        }

        public async Task<GetProjectResponse> Handle(GetProjectRequest request, CancellationToken token)
        {
            var result = new GetProjectResponse();

            var entity = await Repository.GetProject(request.Id);

            if (entity != null)
            {
                result.Project = new Project
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description
                };

                result.Status = true;
            }

            return result;
        }
    }
}