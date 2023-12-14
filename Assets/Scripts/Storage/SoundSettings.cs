using CycladeStorage;

namespace CycladeUIExample.Storage
{
    public class SoundSettings : IStorageSection
    {
        public bool IsMusicEnabled;

        public SoundSettings()
        {
            Reset();
        }

        public void Reset()
        {
            IsMusicEnabled = true;
        }
    }
}