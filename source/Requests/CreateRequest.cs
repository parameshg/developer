using System.ComponentModel.DataAnnotations;

namespace Developer.Api.Requests
{
    public class CreateRequest
    {
        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        public int Value { get; set; } = 0;
    }
}