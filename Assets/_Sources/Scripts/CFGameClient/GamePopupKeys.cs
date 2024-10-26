using CerberusFramework.Utilities;

namespace CFGameClient
{
    public record GamePopupKeys : Enumeration<GamePopupKeys>
    {
        public static GamePopupKeys PausePopup = new(1000, nameof(PausePopup));
        public static GamePopupKeys WinPopupVariant = new(1001, nameof(WinPopupVariant));
        public static GamePopupKeys FailPopupVariant = new(1002, nameof(FailPopupVariant));

        protected GamePopupKeys(int id, string name) : base(id, name)
        {
        }
    }
}
