namespace Developer.Api.Domain
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public class Entity : IEntity
    {
        public Guid Id { get; set; }
    }
}