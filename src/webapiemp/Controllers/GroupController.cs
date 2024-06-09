using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapiemp.DTOs.RequestDTO;
using webapiemp.Extensions;
using webapiemp.Models;
using webapiemp.Services;

namespace webapiemp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthServices _authServices;
    public GroupController(ApplicationDbContext context, IAuthServices authServices)
    {
        _context = context;
        _authServices = authServices;
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateGroup([FromBody] NewGroupDTO groupInfo)
    {
        var newGroup = new Group
        {
            Name = groupInfo.Name,
            InvitationCode = Guid.NewGuid().ToString(),
            AuthorId = HttpContext.GetUserId(),
            Author = _context.Users.Single(x => x.Id == HttpContext.GetUserId())
        };

        var group = await _context.Groups.AddAsync(newGroup);
        await _context.SaveChangesAsync();

        var newGroupMembership = new GroupMembership
        {
            UserId = HttpContext.GetUserId(),
            GroupId = group.Entity.Id,
            Role = "super_admin",
            Status = "active"
        };

        await _context.GroupMemberships.AddAsync(newGroupMembership);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> RemoveGroup([FromRoute] int id)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group == null) 
        { 
            return NotFound();
        }

        if (!_authServices.IsSuperAdmin(HttpContext.GetUserId(), id))
        {
            return Unauthorized("You are not authorized");
        }
        

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return Ok();
    }

    


}
