using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Command.CreateTag
{
    public record CreateTagCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int CountUses { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
    {
        private readonly IApplicationDbContext _context;
        public CreateTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var existingTag = await _context.Tags
             .FirstOrDefaultAsync(t => t.Name == request.Name && t.UserId == request.UserId &&t.IsActive, cancellationToken);

            if (existingTag != null)
            {
                throw new InvalidOperationException("A tag with the same name already exists.");
            }
            var entity = new Tag
            {
                Name = request.Name,
                UserId = request.UserId,
                CountUses = request.CountUses,
                IsActive = true,
                Created = DateTime.Now,
            };

            _context.Tags.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
