using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.ItemTags.Command.Delete.DeleteItemTagWithIds
{
    public record DeleteItemTagWithIdsCommand(int itemId, int tagId): IRequest;

    public class DeleteItemTagWithIdsCommandHandler : IRequestHandler<DeleteItemTagWithIdsCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteItemTagWithIdsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(DeleteItemTagWithIdsCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ItemTags.Where(x => x.IsActive && x.TodoItemId == request.itemId &&  x.TagId ==request.tagId)
               .FirstOrDefaultAsync(cancellationToken);


            var tag = await _context.Tags.Where(x => x.IsActive && x.Id == request.tagId)
             .FirstOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                throw new NotFoundException(nameof(ItemTag), tag.Id);
            }
            if (tag.CountUses > 0)
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
