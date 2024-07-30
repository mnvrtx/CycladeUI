using System.IO;
using System.Text;
using Shared.Utils.Logging;

namespace Shared.Utils
{
    public static class FileHelper
    {
        private static readonly Log log = new(nameof(FileHelper));

        /// <param name="targetExtension">for example: ".json"</param>
        public static bool IsValidPath(string path, out string errorMessage, string targetExtension = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                errorMessage = $"Error in path \"{path}\". It's null or empty.";
                return false;
            }

            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                errorMessage = "Error in path \"{path}\". It contains invalid characters.";
                return false;
            }

            try
            {
                // This checks the general format of the path
                var checkPath = Path.GetFullPath(path);
            }
            catch (System.Exception e)
            {
                errorMessage = $"Error in path \"{path}\". It's incorrectly formatted. Details:\n{e}";
                return false;
            }

            if (!string.IsNullOrEmpty(targetExtension))
            {
                string extension = Path.GetExtension(path);
                if (string.IsNullOrEmpty(extension) || extension.ToLower() != targetExtension)
                {
                    errorMessage = $"Error in path \"{path}\". Invalid file extension. Expected {targetExtension}";
                    return false;
                }
            }

            errorMessage = "";
            return true;
        }
        
        public static string WriteTextUtf8(string directory, string fileName, string contents)
        {
            var finalPath = Path.GetFullPath($"{directory}/{fileName}");
            return WriteTextUtf8(finalPath, contents);
        }
        
        public static string WriteTextUtf8(string filePath, string contents)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                log.Debug($"Created directory at {directory}");
            }

            File.WriteAllText(filePath, contents, Encoding.UTF8);

            return filePath;
        }
    }
}