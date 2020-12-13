using System.ComponentModel.DataAnnotations;

namespace NetMarket.ValidationAttributes
{
    /// <summary>
    /// Класс валидации логина
    /// </summary>
    public class LoginLengthAttribute : ValidationAttribute
    {
        public LoginLengthAttribute() { }

        public override bool IsValid(object value)
        {
            if (value != null && value.ToString().Length <= 12)
            {
                return true;
            }
            return false;
        }
    }
}