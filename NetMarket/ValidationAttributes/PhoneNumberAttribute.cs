using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NetMarket.ValidationAttributes
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public PhoneNumberAttribute() { }

        public override bool IsValid(object value)
        {
            string stringValue = value.ToString();
            if (stringValue != null)
            {
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