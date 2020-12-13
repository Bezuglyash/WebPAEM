using System.Text.RegularExpressions;

namespace NetMarket.Validations
{
    /// <summary>
    /// Класс проверки правильности ввода номера телефона
    /// </summary>
    public static class PhoneNumberValidation
    {
        public static bool IsValid(string phoneNumber)
        {
            Regex regexFirstFormat = new Regex(@"^[8]{1}[0-9]{10}$");
            MatchCollection matchesFirstFormat = regexFirstFormat.Matches(phoneNumber);
            Regex regexSecondFormat = new Regex(@"^[+]?[7]{1}[0-9]{10}$");
            MatchCollection matchesSecondFormat = regexSecondFormat.Matches(phoneNumber);
            if (matchesFirstFormat.Count == 1 || matchesSecondFormat.Count == 1)
            {
                return true;
            }

            return false;
        }
    }
}