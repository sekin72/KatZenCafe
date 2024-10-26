using CerberusFramework.Utilities;

namespace CFGameClient
{
    public record GameSoundKeys : Enumeration<GameSoundKeys>
    {
        public static GameSoundKeys MainTheme = new(1000, nameof(MainTheme));

        protected GameSoundKeys(int id, string name) : base(id, name)
        {
        }
    }
}