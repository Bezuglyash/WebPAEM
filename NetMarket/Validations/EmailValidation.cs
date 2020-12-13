using System.Text.RegularExpressions;

namespace NetMarket.Validations
{
    /// <summary>
    /// Класс проверки правильности ввода Email
    /// </summary>
    public static class EmailValidation
    {
        public static bool IsValid(string email)
        {
            Regex regexFormat = new Regex(@"^[0-9a-zA-Z]+[@]{1}[a-zA-Z]+[.]{1}[a-zA-Z]+$");
            MatchCollection matchesFormat = regexFormat.Matches(email);
            if (matchesFormat.Count == 1)
            {
                return true;
            }
            return false;
        }
    }
}