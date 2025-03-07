using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.ItemTags.Command.Create.CreateItemTag
{
    public record CreateItemTagCommand : IRequest<int>
    {
        public int TodoItemId { get; set; }
        public int TagId { get; set; }

    }

    public class CreateItemTagCommandHandler : IRequestHandler<CreateItemTagCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateItemTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateItemTagCommand request, CancellationToken cancellationToken)
        {
            var entity = new ItemTag
            {
                TodoItemId = request.TodoItemId,
                TagId = request.TagId,
                Created = DateTime.Now,
                IsActive = true
            };

            _context.ItemTags.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
            
}
