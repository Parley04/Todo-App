using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.ItemTags.Command.Update.UpdateItemTag
{
    public record UpdateItemTagCommand:IRequest
    {
        public int Id { get; set; }
        public int ItemsId { get; set; }
        public int TagId { get; set; }
    }

    public class UpdateItemTagCommandHandler: IRequestHandler<UpdateItemTagCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateItemTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle (UpdateItemTagCommand request , CancellationToken cancellationToken)
        {
            var entity= await _context.ItemTags.Where(x => x.IsActive && x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(ItemTag), request.Id);
            }
            entity.TagId=request.TagId;
            entity.ItemsId = request.ItemsId;
            entity.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
