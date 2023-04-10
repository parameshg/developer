using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Developer.Api.Requests
{
    public class ViewRequest
    {
        [Required]
        [StringLength(32)]
        [FromRoute(Name = "name")]
        public string Name { get; set; }
    }
}