using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.Tags.Command.CreateTag;
using Todo_App.Application.Tags.Command.Delete.DeleteTag;
using Todo_App.Application.Tags.Command.Update.UpdateTag;
using Todo_App.Application.Tags.Queries.GetChosenTags;
using Todo_App.Application.Tags.Queries.GetUnchosenTags;
using Todo_App.Domain.Entities;

namespace Todo_App.WebUI.Controllers;

public class TagsController : ApiControllerBase
{
    [HttpGet("{userId}/{todoItemId}")]
    public async Task<ActionResult<List<Tag>>> GetUnchosenTags(string userId, int todoItemId)
    {
        var result = await Mediator.Send(new GetUnchosenTagsQuery { UserId = userId, TodoItemId = todoItemId });
        return Ok(result);
    }
    [HttpGet("ChosenTags/{userId}/{todoItemId}")]
    public async Task<ActionResult<List<Tag>>> GetChosenTags(string userId, int todoItemId)
    {
        var result = await Mediator.Send(new GetChosenTagsQuery { UserId = userId, TodoItemId = todoItemId });
        return Ok(result);
    }


    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTagCommand command)
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
        await Mediator.Send(new DeleteTagCommand(id));

        return NoContent();
    }
}
