using webapiemp.Models;

namespace webapiemp.Services;

public interface IAuthServices
{
    Task<bool> IsAuthorAsync(int userId, int cardId);
    Task<bool> IsAssigneeAsync(int userId, int cardId);
    bool IsAdmin(int userId, int groupId);
    bool IsSuperAdmin(int userId, int groupId);
    Task<bool> IsMember(int userId, int groupId);
    Task<GroupMembership> GetMemberAsync(int userId, int groupId);
    int JWTId(HttpContext httpContext);
}
