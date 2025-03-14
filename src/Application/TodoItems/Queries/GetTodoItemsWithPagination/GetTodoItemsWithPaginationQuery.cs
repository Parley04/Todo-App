using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Common.Models;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public string UserId { get; init; } = string.Empty;
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoItems
         .Where(x => x.ListId == request.ListId && x.IsActive)
         .OrderBy(x => x.Title)
         .Include(x => x.ItemTags.Where(it => it.IsActive)) 
         .ThenInclude(it => it.Tag); 

        var mappedItems = await query
            .Select(item => new TodoItemBriefDto
            {
                Id = item.Id,
                Title = item.Title,
                Done = item.Done,
                Tags = item.ItemTags
                    .Where(it => it.Tag.UserId == request.UserId)
                    .Select(it => new ItemTag
                    {
                        Tag = new Tag
                        {
                            Name = it.Tag.Name,
                            UserId = it.Tag.UserId
                        }
                    })
                    .ToList()
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return mappedItems;

    }
}
