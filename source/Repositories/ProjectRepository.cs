using Amazon.DynamoDBv2.DataModel;
using Developer.Api.Domain;
using EnsureThat;

namespace Developer.Api.Repositories
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetProjects();

        Task<Project> GetProject(Guid id);

        Task<bool> CreateProject(Guid id, string? name, string? description);

        Task<bool> UpdateProject(Guid id, string? name, string? description);

        Task<bool> DeleteProject(Guid id);
    }

    public class ProjectRepository : IProjectRepository
    {
        [DynamoDBTable("projects")]
        private class Entity
        {
            [DynamoDBHashKey]
            [DynamoDBProperty("id")]
            public string? Id { get; set; }

            [DynamoDBProperty("name")]
            public string? Name { get; set; }

            [DynamoDBProperty("description")]
            public string? Description { get; set; }
        }

        private IDynamoDBContext db;

        public ProjectRepository(IDynamoDBContext context)
        {
            db = EnsureArg.IsNotNull(context);
        }

        public async Task<List<Project>> GetProjects()
        {
            var result = new List<Project>();

            var conditions = new List<ScanCondition>();

            var entities = await db.ScanAsync<Entity>(conditions).GetRemainingAsync();

            if (entities != null)
            {
                foreach(var entity in entities)
                {
                    result.Add(new Project
                    {
                        Id = Guid.Parse(entity.Id),
                        Name = entity.Name,
                        Description = entity.Description
                    });
                }
            }

            return result;
        }

        public async Task<Project> GetProject(Guid id)
        {
            var result = new Project();

            var entity = await db.LoadAsync<Entity>(id.ToString());

            if (entity != null)
            {
                result = new Project
                {
                    Id = Guid.Parse(entity.Id),
                    Name = entity.Name,
                    Description = entity.Description
                };
            }

            return result;
        }

        public async Task<bool> CreateProject(Guid id, string? name, string? description)
        {
            var result = false;

            await db.SaveAsync(new Entity { Id = id.ToString(), Name = name, Description = description });

            result = true;

            return result;
        }

        public async Task<bool> UpdateProject(Guid id, string? name, string? description)
        {
            var result = false;

            await db.SaveAsync(new Entity { Id = id.ToString(), Name = name, Description = description });

            result = true;

            return result;
        }

        public async Task<bool> DeleteProject(Guid id)
        {
            var result = false;

            await db.DeleteAsync<Entity>(id.ToString());

            result = true;

            return result;
        }
    }
}