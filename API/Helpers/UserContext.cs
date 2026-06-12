using System.Security.Claims;

namespace API.Helpers;

public static class UserContext
{
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
