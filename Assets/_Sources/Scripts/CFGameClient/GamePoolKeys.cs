using CerberusFramework.Utilities;

namespace CFGameClient
{
    public record GamePoolKeys : Enumeration<GamePoolKeys>
    {
        public static GamePoolKeys ExternalParticlesSystemView = new(500, nameof(ExternalParticlesSystemView));

        public static GamePoolKeys Board = new(600, nameof(Board));
        public static GamePoolKeys Sheet = new(601, nameof(Sheet));
        public static GamePoolKeys Container = new(602, nameof(Container));
        public static GamePoolKeys Tile = new(603, nameof(Tile));

        public static GamePoolKeys PausePopup = new(1000, nameof(PausePopup));
        public static GamePoolKeys WinPopupVariant = new(1001, nameof(WinPopupVariant));
        public static GamePoolKeys FailPopupVariant = new(1002, nameof(FailPopupVariant));

        protected GamePoolKeys(int id, string name) : base(id, name)
        {
        }
    }
}