using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using webapiemp.Models;

namespace webapiemp.Services;

public class AuthServices : IAuthServices
{
    private readonly ApplicationDbContext _context;
    public AuthServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsAuthorAsync(int userId, int cardId)
    {
        var card = await _context.Cards.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cardId);
        if (card == null) return false;
        if (card.AuthorId != userId) return false;
        return true;
    }

    public async Task<bool> IsAssigneeAsync(int userId, int cardId)
    {
        var card = await _context.Cards.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cardId);
        if (card == null) return false;
        if (card.ResponderId != userId) return false;
        return true;
    }

    public bool IsAdmin(int userId, int groupId)
    {
        var member = (from u in _context.Users
                      join m in _context.GroupMemberships
                      on u.Id equals m.UserId
                      where u.Id == userId && (m.Role == "super_admin" || m.Role == "admin") && m.GroupId == groupId
                      select new
                      {
                          Role = m.Role,
                      }
                      );
        if (member == null || member.Count() == 0) return false;
        return true;
    }

    public bool IsSuperAdmin(int userId, int groupId)
    {
        var member = (from u in _context.Users
                      join m in _context.GroupMemberships
                      on u.Id equals m.UserId
                      where u.Id == userId && m.Role == "super_admin" && m.GroupId == groupId
                      select new
                      {
                          Role = m.Role,
                      }
                      );
        if (member == null || member.Count() == 0) return false;
        return true;
    }

    public async Task<bool> IsMember(int userId, int groupId)
    {
        var member = await _context.GroupMemberships.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
        if (member == null) return false;

        return true;
    }

    public async Task<GroupMembership> GetMemberAsync(int userId, int groupId)
    {
        var member = await _context.GroupMemberships.FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
        if (member == null) return null;

        return member;
    }

    public int JWTId(HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (userId == null)
            return -1;
        else
            return Int32.Parse(userId);
    }



}
