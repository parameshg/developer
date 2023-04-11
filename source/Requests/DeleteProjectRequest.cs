using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Developer.Api.Requests
{
    public class DeleteProjectRequest
    {
        [Required]
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }

        [FromBody]
        [Required]
        public bool Confirm { get; set; } = false;
    }
}