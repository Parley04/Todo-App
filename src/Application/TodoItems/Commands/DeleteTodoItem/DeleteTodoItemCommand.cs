using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Events;

namespace Todo_App.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        var listId = entity.ListId;

        var userId = await _context.TodoLists
            .Where(l => l.Id == listId)
            .Select(l => l.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId == null)
        {
            throw new NotFoundException("User not found for the given TodoList.");
        }

        var userTags = await _context.Tags
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);

        var tagIds = userTags.Select(t => t.Id).ToList();
        var itemTags = await _context.ItemTags
            .Where(it => it.TodoItemId == request.Id && tagIds.Contains(it.TagId))
            .ToListAsync(cancellationToken);

        var filteredTagIds = itemTags.Select(it => it.TagId).Distinct().ToList();
        var filteredTags = await _context.Tags
            .Where(t => filteredTagIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        foreach (var tag in filteredTags)
        {
            tag.CountUses = Math.Max(0, tag.CountUses - 1);
        }
        entity.IsActive = false;
        entity.LastModified = DateTime.Now;
        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }


}
