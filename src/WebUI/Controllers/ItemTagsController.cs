using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.ItemTags.Command.Create.CreateItemTag;
using Todo_App.Application.ItemTags.Command.Delete.DeleteItemTag;
using Todo_App.Application.ItemTags.Command.Update.UpdateItemTag;
using Todo_App.Application.Tags.Command.Delete.DeleteTag;

namespace Todo_App.WebUI.Controllers;

public class ItemTagsController : ApiControllerBase
{
    //[HttpGet("{userId}/{todoItemId}")]
    //public async Task<ActionResult<List<Tag>>> Get(string userId, int todoItemId)
    //{
    //    var result = await Mediator.Send(new GetUnchosenTagsQuery { UserId = userId, TodoItemId = todoItemId });
    //    return Ok(result);
    //}

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateItemTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("{id}")]
    public async Task<ActionResult> Update(int id, UpdateItemTagCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteItemTagCommand(id));

        return NoContent();
    }
}
