using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoLists.Queries.GetTodos
{
    public class ItemTagDto : IMapFrom<ItemTag>
    {
        public ItemTagDto()
        {
            Tags = new List<TagDto>();
        }

        public int TodoItemId { get; set; }
        public int TagId { get; set; }
        public IList<TagDto>? Tags { get;  set; } = new List<TagDto>();
    }
}
