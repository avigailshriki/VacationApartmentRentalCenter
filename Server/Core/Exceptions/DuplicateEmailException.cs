using System;

namespace Core.Exceptions
{
    // נזרקת כאשר מנסים להירשם עם אימייל שכבר קיים במערכת.
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string message) : base(message)
        {
        }
    }
}
