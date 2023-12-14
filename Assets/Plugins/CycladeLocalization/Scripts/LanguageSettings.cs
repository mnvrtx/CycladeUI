using CycladeStorage;

namespace CycladeLocalization
{
    public class LanguageSettings : IStorageSection
    {
        public string Language;

        public LanguageSettings()
        {
            Reset();
        }

        public void Reset()
        {
            Language = "";
        }
    }
}