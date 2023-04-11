namespace Developer.Api.Domain
{
    public class License : Entity
    {
        public Guid Authority { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Type { get; set; }

        public DateTime Expiry { get; set; }

        public string? Data { get; set; }
    }
}