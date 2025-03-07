namespace Todo_App.Domain.Entities
{
    public class ItemTag:BaseAuditableEntity
    {
        public int TodoItemId { get; set; }
        public TodoItem TodoItem { get; set; } 

        public int TagId{ get; set; }
        public Tag Tag { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
