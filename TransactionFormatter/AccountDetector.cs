namespace TransactionFormatter
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class AccountDetector
    {
        private static readonly Dictionary<string, string> Accounts = new();

        private static readonly Dictionary<Regex, string> Descriptions = new()
        {
            {new Regex("^.+金拱门.+$", RegexOptions.Compiled), "餐饮"},
            {new Regex("^.+板栗.+$", RegexOptions.Compiled), "餐饮"},
            {new Regex("^.+纯味.+$", RegexOptions.Compiled), "餐饮"},
            {new Regex("^.+地铁.+$", RegexOptions.Compiled), "交通"},
            {new Regex("^.+交通卡.+$", RegexOptions.Compiled), "交通"},
            {new Regex("^.+超巿.+$", RegexOptions.Compiled), "日用"},
        };

        public static string ResolveAccount(string abbr)
        {
            return Accounts.TryGetValue(abbr, out var account) ? account : abbr;
        }

        public static string ResolveDescription(string desc)
        {
            foreach (var (regex, account) in Descriptions)
            {
                if (regex.IsMatch(desc))
                {
                    return account;
                }
            }

            return desc;
        }
    }
}