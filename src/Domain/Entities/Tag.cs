namespace Todo_App.Domain.Entities
{
    public class Tag : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int CountUses { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
