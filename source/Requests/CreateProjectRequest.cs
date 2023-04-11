using System.ComponentModel.DataAnnotations;

namespace Developer.Api.Requests
{
    public class CreateProjectRequest
    {
        [Required]
        [StringLength(32)]
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}