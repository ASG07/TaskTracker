using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapiemp.DTOs;
using webapiemp.Extensions;
using webapiemp.Models;
using webapiemp.Services;

namespace webapiemp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupMembershipController : Controller
{

    private readonly ApplicationDbContext _context;
    private readonly IAuthServices _authServices;
    public GroupMembershipController(ApplicationDbContext context, IAuthServices authServices)
    {
        _context = context;
        _authServices = authServices;
    }

    [HttpGet("{groupId}")]
    [Authorize]
    public async Task<ActionResult<List<memberDTO>>> GetMembersByGroupId(int groupId)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return BadRequest();

        if (await _authServices.IsMember(userId, groupId) == false) return Unauthorized();

        var groupMembers = from m in _context.GroupMemberships
                           join user in _context.Users
                            on m.UserId equals user.Id
                           join g in _context.Groups
                            on m.GroupId equals g.Id
                           where m.GroupId == groupId && m.Status != "deleted"
                           select new memberDTO
                           {
                               UserId = user.Id,
                               Name = user.UserName,
                               role = m.Role,
                               AmountWorkingOn = m.AmountWorkingOn,
                               AmountFinished = m.AmountFinished,
                               AmountBugsSubmitted = m.AmountBugsSubmitted
                           };
        return groupMembers.ToList();

    }

    [HttpDelete("{groupId}/{userId}")]
    [Authorize]
    public async Task<ActionResult> DeleteMember(int groupId, int userId)
    {

        //get userId from token
        var JWTId = HttpContext.GetUserId();
        if (JWTId == -1) return BadRequest("Token is tampered with");

        //check if user is admin
        if (_authServices.IsAdmin(JWTId, groupId) == false) return Unauthorized("you are not authorized to remove a member");

        var admin = await _context.GroupMemberships.FirstOrDefaultAsync(m => m.UserId == JWTId && m.GroupId == groupId);
        if (admin == null) return NotFound();

        //get member
        var deletedMember = await _context.GroupMemberships.FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
        if (deletedMember == null) return NotFound();


        if (deletedMember.Role == "super_admin")
            return BadRequest("you can not kick the group author");

        if (admin.Role == "admin")
            if (deletedMember.Role == "admin")
                return BadRequest("you can not kick an admin");

        //var removedUser = _context.GroupMemberships.Remove(deletedMember);
        deletedMember.Status = "deleted";
        await _context.SaveChangesAsync();
        return Ok("Removed" + deletedMember.ToString()); //exposes implementation
    }

    [HttpPost("{groupId}")]
    [Authorize]
    public async Task<ActionResult> AddNewMember(int groupId, [FromBody] string NewMemberEmail)
    {
        //get userId from token
        var JWTId = HttpContext.GetUserId();
        if (JWTId == -1) return BadRequest("Token is tampered with");

        //check if user is admin
        if (_authServices.IsAdmin(JWTId, groupId) == false) return Unauthorized("you are not authorized to add a member");

        //get memeber
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == NewMemberEmail);
        if (user == null) return NotFound();

        var existingMember = await _context.GroupMemberships.FirstOrDefaultAsync(x => x.UserId == user.Id && x.GroupId == groupId);
        if (existingMember != null) { 
            
            if (existingMember.Status == "deleted") {
                existingMember.Status = "active";
                await _context.SaveChangesAsync();
                return Ok();
            }
            else return BadRequest("user is already a member");
                
        }
        

        await _context.GroupMemberships.AddAsync(new GroupMembership { UserId = user.Id, GroupId = groupId, Role = "member", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0, Status = "active" });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{groupId}/leave")]
    [Authorize]
    public async Task<ActionResult> ExitGroup(int groupId)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return BadRequest();

        if (!await _authServices.IsMember(userId, groupId)) return NotFound();

        var member = await _authServices.GetMemberAsync(userId, groupId);
        if (member == null) return NotFound();

        //if super admin want to leave, someone else has to take the super_admin role after him preferably an admin, if no other member exist, the group will be deleted
        if (member.Role == "super_admin")
        {
            var adminToSuperAdmin = await _context.GroupMemberships.FirstOrDefaultAsync(x => x.Role == "admin" && x.GroupId == groupId);
            if (adminToSuperAdmin == null)
            {
                var memberToSuperAdmin = await _context.GroupMemberships.FirstOrDefaultAsync(x => x.Role == "member" && x.GroupId == groupId);
                if (memberToSuperAdmin == null)
                {
                    var exitedGroup = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
                    if (exitedGroup == null) return NotFound();
                    _context.Groups.Remove(exitedGroup);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    memberToSuperAdmin.Role = "super_admin";
                }
            }
            else
            {
                adminToSuperAdmin.Role = "super_admin";
            }
            await _context.SaveChangesAsync();
        }


        //_context.GroupMemberships.Remove(member);
        member.Status = "deleted";
        member.Role = "member";
        await _context.SaveChangesAsync();
        return Ok(member);
    }

    [HttpPost("{groupCode}/join")]
    [Authorize]
    public async Task<ActionResult> EnterGroup(String groupCode)
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return BadRequest();

        var group = await _context.Groups.FirstOrDefaultAsync(x => x.InvitationCode == groupCode);
        if (group == null) return NotFound("Invitation code is wrong");

        if (await _authServices.IsMember(userId, group.Id)) return BadRequest("You are already a member");

        await _context.GroupMemberships.AddAsync(new GroupMembership { UserId = userId, GroupId = group.Id, Role = "member", AmountBugsSubmitted = 0, AmountFinished = 0, AmountWorkingOn = 0 });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{groupId}/makeMemberAdmin/{userId}")]
    [Authorize]
    public async Task<ActionResult> MakeMemberAdmin(int groupId, int userId)
    {
        var JWTId = HttpContext.GetUserId();
        if (JWTId == -1) return BadRequest();

        if (!_authServices.IsSuperAdmin(JWTId, groupId)) return BadRequest();

        var member = await _authServices.GetMemberAsync(userId, groupId);
        if (member == null) return NotFound();

        if (JWTId == userId) return BadRequest();
        if (member.Role == "admin") return BadRequest();

        member.Role = "admin";
        await _context.SaveChangesAsync();
        return Ok(member);
    }

    [HttpPut("{groupId}/makeAdminMember/{userId}")]
    [Authorize]
    public async Task<ActionResult> MakeAdminMember(int groupId, int userId)
    {
        var JWTId = HttpContext.GetUserId();
        if (JWTId == -1) return BadRequest();

        if (!_authServices.IsSuperAdmin(JWTId, groupId)) return BadRequest();

        var member = await _authServices.GetMemberAsync(userId, groupId);
        if (member == null) return NotFound();

        if (JWTId == userId) return BadRequest();
        if (member.Role == "member") return BadRequest();

        member.Role = "member";
        await _context.SaveChangesAsync();
        return Ok(member);
    }

    [HttpGet()]
    [Authorize]
    public async Task<ActionResult> GetGroups()
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return Forbid();

        //var member = _context.GroupMemberships.AsNoTracking().Where(x => x.UserId == userId);
        var member = from m in _context.GroupMemberships
                     join g in _context.Groups
                        on m.GroupId equals g.Id
                     where m.UserId == userId
                     select new
                     {
                         groupId = g.Id,
                         groupName = g.Name,
                         role = m.Role,
                         AmountWorkingOn = m.AmountWorkingOn,
                         AmountFinished = m.AmountFinished,
                         AmountBugsSubmitted = m.AmountBugsSubmitted
                     };

        return Ok(member);
    }

    
}
