using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.ItemTags.Command.Delete.DeleteItemTag
{
    public record DeleteItemTagCommand(int Id) : IRequest;

    public class DeleteItemTagHandler:IRequestHandler<DeleteItemTagCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteItemTagHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteItemTagCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ItemTags.Where(x => x.IsActive && x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(ItemTag), request.Id);
            }

            var tag = await _context.Tags.Where(x => x.IsActive && x.Id == entity.TagId)
             .FirstOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                throw new NotFoundException(nameof(ItemTag), tag.Id);
            }
            if(tag.CountUses > 0)
            {
                tag.CountUses--;
                tag.LastModified = DateTime.Now;
            }
           
            entity.IsActive = false;
            entity.LastModified = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
