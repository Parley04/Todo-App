using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Tags.Queries.GetUnchosenTags;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Queries.GetChosenTags
{
    public record GetChosenTagsQuery : IRequest<List<Tag>>
    {
        public string UserId { get; init; } = string.Empty;
        public int TodoItemId { get; set; }
        public class GetChosenTagsQueryHandler : IRequestHandler<GetChosenTagsQuery, List<Tag>>
        {

            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetChosenTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Tag>> Handle(GetChosenTagsQuery request, CancellationToken cancellationToken)
            {
                var chosenTags = await _context.ItemTags
                    .Where(it => it.TodoItemId == request.TodoItemId && it.IsActive) 
                    .Select(it => it.Tag) 
                    .Where(tag => tag != null && tag.UserId == request.UserId && tag.IsActive) 
                    .OrderByDescending(t => t.CountUses) 
                    .ToListAsync(cancellationToken);

                return chosenTags;
            }

        }
    }
}
