using CycladeStorage;

namespace Solonity.UnityDef.Storage
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