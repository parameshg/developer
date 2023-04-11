using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Developer.Api.Requests
{
    public class ViewProjectRequest
    {
        [Required]
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}