namespace Shared.Utils.Logging
{
    public static class LogHelpers
    {
        public static string MarkImportant(this string t, bool flag)
        {
            if (!flag)
                return t;
            t = $"(IMPORTANT) {t}";
            return t;
        }
        
        public static string MarkOrange(this string t, bool flag)
        {
            if (!flag)
                return t;
            t = $"<color=orange>{t}</color>";
            return t;
        }
    }
}