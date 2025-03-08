using AutoMapper;
using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public class TodoItemDto : IMapFrom<TodoItem>
{
    public TodoItemDto()
    {
        ItemTags = new List<ItemTagDto>();
    }
    public int Id { get; set; }

    public int ListId { get; set; }

    public string? Title { get; set; }

    public bool Done { get; set; }

    public PriorityLevel Priority { get; set; }

    public string? Note { get; set; }

    public IList<ItemTagDto>? ItemTags { get;  set; } = new List<ItemTagDto>();


    public void Mapping(Profile profile)
    {
        profile.CreateMap<TodoItem, TodoItemDto>()
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => (int)s.Priority));
    }
}
