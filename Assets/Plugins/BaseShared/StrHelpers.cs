using System.Text;
using Shared.Utils.Logging;

namespace BaseShared
{
    public static class StrHelpers
    {
        private static readonly Log log = new(nameof(StrHelpers));
        public static string WrapText(this string text, int maxLineLength)
        {
            var sb = new StringBuilder();
            int currentLength = 0;

            foreach (var word in text.Split(' '))
            {
                // log.Info($"word: `{word}`. currentLength: {currentLength}");
                var containsN = word.Contains('\n');
                if (containsN)
                    currentLength = 0;

                if (currentLength + word.Length > maxLineLength)
                {
                    sb.AppendLine();
                    currentLength = 0;
                }

                sb.Append(word + " ");
                if (!containsN)
                    currentLength += word.Length + 1;
            }

            return sb.ToString();
        }
    }
}