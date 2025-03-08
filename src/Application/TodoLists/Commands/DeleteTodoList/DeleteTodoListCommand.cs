using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoLists.Commands.DeleteTodoList;

public record DeleteTodoListCommand(int Id) : IRequest;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoList), request.Id);
        }

        entity.IsActive = false;
        entity.LastModified = DateTime.Now;
       // _context.TodoLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
    //public async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    //{
    //    var entity = await _context.TodoLists
    //        .FindAsync(new object[] { request.Id }, cancellationToken);

    //    if (entity == null)
    //    {
    //        throw new NotFoundException(nameof(TodoList), request.Id);
    //    }

    //    var listId = entity.Id;

    //    var todoItems = await _context.TodoItems
    //        .Where(ti => ti.ListId == listId)
    //        .ToListAsync(cancellationToken);

    //    if (!todoItems.Any())
    //    {
    //        _context.TodoLists.Remove(entity);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return Unit.Value;
    //    }

    //    var todoItemIds = todoItems.Select(ti => ti.Id).ToList();

    //    var userId = entity.UserId;

    //    if (userId == null)
    //    {
    //        throw new NotFoundException("User not found for the given TodoList.");
    //    }

    //    var userTags = await _context.Tags
    //        .Where(t => t.UserId == userId)
    //        .ToListAsync(cancellationToken);

    //    var tagIds = userTags.Select(t => t.Id).ToList();

    //    var itemTags = await _context.ItemTags
    //        .Where(it => todoItemIds.Contains(it.TodoItemId) && tagIds.Contains(it.TagId))
    //        .ToListAsync(cancellationToken);

    //    var filteredTagIds = itemTags.Select(it => it.TagId).Distinct().ToList();

    //    var filteredTags = await _context.Tags
    //        .Where(t => filteredTagIds.Contains(t.Id))
    //        .ToListAsync(cancellationToken);

    //    foreach (var tag in filteredTags)
    //    {
    //        tag.CountUses = Math.Max(0, tag.CountUses - 1);
    //    }

    //    await _context.SaveChangesAsync(cancellationToken);

    //    _context.TodoLists.Remove(entity);
    //    await _context.SaveChangesAsync(cancellationToken);

    //    return Unit.Value;
    //}

}
