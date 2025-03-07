using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Queries.GetUnchosenTags
{
    public record GetUnchosenTagsQuery : IRequest<List<Tag>>
    {
        public string UserId { get; init; }
        public int TodoItemId { get; set; }

        public class GetUnchosenTagsQueryHandler : IRequestHandler<GetUnchosenTagsQuery, List<Tag>>
        {

            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetUnchosenTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<Tag>> Handle(GetUnchosenTagsQuery request, CancellationToken cancellationToken)
            {
                var unchosenTags = await _context.Tags
                    .Where(tag => tag.UserId == request.UserId && tag.IsActive)
                    .Where(tag => !_context.ItemTags
                        .Any(it => it.TagId == tag.Id && it.TodoItemId == request.TodoItemId && it.IsActive))
                    .OrderByDescending(t=>t.CountUses)
                    .ToListAsync(cancellationToken);

                return unchosenTags;
            }
        }
    }
}
