﻿using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public class TodoListDto : IMapFrom<TodoList>
{
    public TodoListDto()
    {
        Items = new List<TodoItemDto>();
    }

    public int Id { get; set; }

    public string? Title { get; set; }
    public string UserId { get; set; } = string.Empty;

    public Colour? Colour { get; set; } = Colour.White;


    public IList<TodoItemDto>? Items { get;  set; } = new List<TodoItemDto>();
}
