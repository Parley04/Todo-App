using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public record GetTodosQuery : IRequest<TodosVm>
{
    public string UserId { get; init; } = string.Empty;
}

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var lists = await _context.TodoLists
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Include(x => x.Items.Where(x => x.IsActive))
                .ThenInclude(item => item.ItemTags.Where(x=>x.IsActive))
                .ThenInclude(it => it.Tag)
            .OrderBy(t => t.Title)
            .ToListAsync(cancellationToken);

        var listDtos = lists.Select(list => new TodoListDto
        {
            Id = list.Id,
            Title = list.Title,
            Colour = list.Colour,
            UserId = list.UserId,
            Items = list.Items.Select(item => new TodoItemDto
            {
                Id = item.Id,
                Title = item.Title,
                ListId=item.ListId,
                Done = item.Done,
                Note = item.Note,
                Priority = item.Priority,
                ItemTags = item.ItemTags.Where(it => it.Tag != null 
                && it.Tag.UserId == request.UserId 
                && it.IsActive 
                && it.Tag.IsActive) 
                        .Select(it => new ItemTagDto
                        {
                        Id=it.Id,
                        TodoItemId = it.TodoItemId,
                        TagId = it.TagId,
                        Tags = it.Tag != null ? new List<TagDto>
                        {
                        new TagDto
                        {
                            Name = it.Tag.Name,
                            UserId = it.Tag.UserId,
                            CountUses = it.Tag.CountUses
                        }} : new List<TagDto>()
                    })
                    .ToList()
            }).ToList()
        }).ToList();

        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),
            Lists = listDtos
        };
    }

}
public class TodoProfile : Profile
{
    public TodoProfile()
    {
        // TodoList -> TodoListDto
        CreateMap<TodoList, TodoListDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        // TodoItem -> TodoItemDto
        CreateMap<TodoItem, TodoItemDto>()
            .ForMember(dest => dest.ItemTags, opt => opt.MapFrom(src => src.ItemTags));

        // ItemTag -> ItemTagDto
        CreateMap<ItemTag, ItemTagDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tag));

        // Tag -> TagDto
        CreateMap<Tag, TagDto>();
    }
}