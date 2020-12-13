using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NetMarket.ValidationAttributes
{
    /// <summary>
    /// Класс валидации номера телефона (не может быть null)
    /// </summary>
    public class PhoneNumberNotNullAttribute : ValidationAttribute
    {
        public PhoneNumberNotNullAttribute() { }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string stringValue = value.ToString();
                Regex regexFirstFormat = new Regex(@"^[8]{1}[0-9]{10}$");
                MatchCollection matchesFirstFormat = regexFirstFormat.Matches(stringValue);
                Regex regexSecondFormat = new Regex(@"^[+]?[7]{1}[0-9]{10}$");
                MatchCollection matchesSecondFormat = regexSecondFormat.Matches(stringValue);
                if (matchesFirstFormat.Count == 1 || matchesSecondFormat.Count == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}