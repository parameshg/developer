using Developer.Api.Repositories;
using Developer.Api.Requests;
using EnsureThat;
using FluentValidation;
using MediatR;

namespace Developer.Api.Handlers
{
    public class DeleteProjectRequestValidator : AbstractValidator<DeleteProjectRequest>
    {
        public DeleteProjectRequestValidator()
        {
            RuleFor(request => request.Id).NotEmpty();
        }
    }

    public class DeleteProjectRequest : IRequest<DeleteProjectResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProjectResponse
    {
        public bool Status { get; set; }
    }

    public class DeleteProjectHandler : IRequestHandler<DeleteProjectRequest, DeleteProjectResponse>
    {
        private IProjectRepository Repository { get; }

        public DeleteProjectHandler(IProjectRepository repository)
        {
            Repository = EnsureArg.IsNotNull(repository);
        }

        public async Task<DeleteProjectResponse> Handle(DeleteProjectRequest request, CancellationToken token)
        {
            var result = new DeleteProjectResponse();

            result.Status = await Repository.DeleteProject(request.Id);

            return result;
        }
    }
}