namespace Developer.Api.Domain
{
    public class Certificate : Entity
    {
        public string? Name { get; set; }

        public string? PrivateKey { get; set; }

        public string? PublicKey { get; set; }
    }
}