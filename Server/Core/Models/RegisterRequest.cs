using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "יש להזין שם מלא")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין מספר טלפון")]
        [Phone(ErrorMessage = "מספר טלפון לא תקין")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין סיסמה")]
        [MinLength(6, ErrorMessage = "הסיסמה חייבת להכיל לפחות 6 תווים")]
        public string Password { get; set; } = string.Empty;
    }
}
