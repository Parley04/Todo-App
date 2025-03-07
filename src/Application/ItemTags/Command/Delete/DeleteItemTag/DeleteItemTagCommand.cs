using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.ItemTags.Command.Delete.DeleteItemTag
{
    public record DeleteItemTagCommand(int Id) : IRequest;

    public class DeleteItemTagCommandHandler : IRequest<DeleteItemTagCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteItemTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<Unit> Handle(DeleteItemTagCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ItemTags
             .Where(x => x.IsActive && x.Id == request.Id)
             .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(ItemTag), request.Id);
            }

            entity.IsActive = false;
            entity.LastModified = DateTime.Now;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }

}
