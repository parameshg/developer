using Developer.Api.Domain;
using Developer.Api.Repositories;
using EnsureThat;
using FluentValidation;
using MediatR;

namespace Developer.Api.Handlers
{
    public class GetProjectsRequestValidator : AbstractValidator<GetProjectsRequest>
    {
        public GetProjectsRequestValidator()
        {
        }
    }

    public class GetProjectsRequest : IRequest<GetProjectsResponse>
    {
    }

    public class GetProjectsResponse
    {
        public List<Project> Projects { get; set; } = new List<Project>();

        public bool Status { get; set; }
    }

    public class GetProjectsHandler : IRequestHandler<GetProjectsRequest, GetProjectsResponse>
    {
        private IProjectRepository Repository { get; }

        public GetProjectsHandler(IProjectRepository repository)
        {
            Repository = EnsureArg.IsNotNull(repository);
        }

        public async Task<GetProjectsResponse> Handle(GetProjectsRequest request, CancellationToken token)
        {
            var result = new GetProjectsResponse();

            var projects = await Repository.GetProjects();

            if (projects != null)
            {
                result.Projects.AddRange(projects);

                result.Status = true;
            }

            return result;
        }
    }
}