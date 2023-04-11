using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Developer.Api.Requests
{
    public class UpdateProjectRequest
    {
        [Required]
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }

        [FromBody]
        [Required]
        [StringLength(32)]
        public string? Name { get; set; }

        [FromBody]
        public string? Description { get; set; }
    }
}