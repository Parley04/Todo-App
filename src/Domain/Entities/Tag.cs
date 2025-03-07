namespace Todo_App.Domain.Entities
{
    public class Tag : BaseAuditableEntity
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public int CountUses { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
