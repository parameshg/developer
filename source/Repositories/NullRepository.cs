using Developer.Api.Domain;
using Developer.Api.Errors;

namespace Developer.Api.Repositories
{
    public class NullRepository : IProjectRepository
    {
        private static readonly string NULL_STORAGE = "No storage configured";

        public Task<List<Project>> GetProjects()
        {
            throw new RepositoryException(NULL_STORAGE);
        }

        public Task<Project> GetProject(Guid id)
        {
            throw new RepositoryException(NULL_STORAGE);
        }

        public Task<bool> CreateProject(Guid id, string? name, string? description)
        {
            throw new RepositoryException(NULL_STORAGE);
        }

        public Task<bool> UpdateProject(Guid id, string? name, string? description)
        {
            throw new RepositoryException(NULL_STORAGE);
        }

        public Task<bool> DeleteProject(Guid id)
        {
            throw new RepositoryException(NULL_STORAGE);
        }
    }
}