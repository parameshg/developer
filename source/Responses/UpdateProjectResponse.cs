using Developer.Api.Domain;

namespace Developer.Api.Responses
{
    public class UpdateProjectResponse
    {
        public Project? Project { get; set; }

        public bool Status { get; set; }
    }
}