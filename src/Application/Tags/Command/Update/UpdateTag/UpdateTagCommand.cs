using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Command.Update.UpdateTag
{
    public class UpdateTagCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int CountUses { get; set; }
    }

    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tags
                .Where(x => x.IsActive && x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Tag), request.Id);
            }

            entity.Name = request.Name;
            entity.UserId = request.UserId;
            entity.CountUses = request.CountUses;
            entity.LastModified = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
