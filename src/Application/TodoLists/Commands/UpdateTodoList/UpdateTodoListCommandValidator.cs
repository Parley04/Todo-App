using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
           .NotEmpty().WithMessage("Title is required.")
           .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
    }

    public async Task<bool> BeUniqueTitle(UpdateTodoListCommand model, string title, CancellationToken cancellationToken)
    {
        return await _context.TodoLists
             .Where(l => l.IsActive && l.Title == title)
             .AllAsync(l => l.Title != title, cancellationToken);
    }
}
