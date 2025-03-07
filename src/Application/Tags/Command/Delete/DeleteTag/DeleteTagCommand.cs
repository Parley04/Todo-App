using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItems.Commands.DeleteTodoItem;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Command.Delete.DeleteTag
{
    public record DeleteTagCommand(int Id) : IRequest;

    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tags
              .Where(x => x.IsActive && x.Id == request.Id)
              .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Tag), request.Id);
            }

            entity.IsActive = false;
            entity.LastModified = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

