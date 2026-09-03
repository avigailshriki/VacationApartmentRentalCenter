using System.Security.Claims;

namespace WebAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // מחזיר את מזהה הבעלים (Owner) המחובר מתוך תביעות ה-JWT, או null אם אין טוקן תקין.
        public static int? GetOwnerId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out var id))
            {
                return id;
            }
            return null;
        }
    }
}
