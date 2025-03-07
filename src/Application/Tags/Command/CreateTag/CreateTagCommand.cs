using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Command.CreateTag
{
    public record CreateTagCommand:IRequest<int>
    {
        public string Name { get; set; }
        public string UserId { get; set; }
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
            var entity = new Tag
            {
                Name = request.Name,
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
