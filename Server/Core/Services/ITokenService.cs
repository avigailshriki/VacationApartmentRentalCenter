namespace Core.Services
{
    public interface ITokenService
    {
        // מנפיק JWT חתום עבור בעלים שהתחבר/נרשם בהצלחה.
        string GenerateToken(int ownerId, string email, string fullName);
    }
}
