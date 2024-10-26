using CerberusFramework.Managers.Pool;
using CerberusFramework.UI.Popups;
using CFGameClient.Core.Scenes;

namespace CFGameClient.UI.Popups.WinPopupVariant
{
    public class WinPopupVariantData : PopupData
    {
        public LevelSceneController LevelSessionScene;

        public WinPopupVariantData(LevelSceneController levelSceneController)
            : base(CFPoolKeys.FromId(GamePoolKeys.WinPopupVariant.Id))
        {
            LevelSessionScene = levelSceneController;
        }
    }
}
