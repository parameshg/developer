using Developer.Api.Domain;

namespace Developer.Api.Responses
{
    public class ListProjectResponse
    {
        public List<Project> Projects { get; set; } = new List<Project>();

        public bool Status { get; set; }
    }
}