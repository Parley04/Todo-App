﻿using Microsoft.EntityFrameworkCore;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Tag> Tags{ get; }
    DbSet<ItemTag> ItemTags{ get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
