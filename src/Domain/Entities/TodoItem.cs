namespace Todo_App.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public int ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }
    public bool IsActive { get; set; } = true;

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value == true && _done == false)
            {                
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }
    public IList<ItemTag> ItemTags { get; private set; } = new List<ItemTag>();

    public TodoList List { get; set; } = null!;
}
