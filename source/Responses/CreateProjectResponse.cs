using Developer.Api.Domain;

namespace Developer.Api.Responses
{
    public class CreateProjectResponse
    {
        public Project? Project { get; set; }

        public bool Status { get; set; }
    }
}