using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    // הבקשה מהקליינט אחרי התחברות/הרשמה מוצלחת מול גוגל - מכילה רק את ה-ID Token
    // שגוגל הנפיקה בדפדפן. השרת מאמת את הטוקן הזה מול גוגל לפני שהוא סומך עליו.
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "חסר Google ID Token")]
        public string IdToken { get; set; } = string.Empty;
    }
}
