using System;
using CerberusFramework.Managers.Pool;
using CerberusFramework.UI.Popups;

namespace CFGameClient.UI.Popups.FailPopupVariant
{
    public class FailPopupVariantData : PopupData
    {
        public Action OnMainMenuButtonClicked;
        public Action OnRestartLevelClicked;

        public FailPopupVariantData(Action onMainMenuButtonClicked, Action onRestartLevelClicked)
            : base(CFPoolKeys.FromId(GamePoolKeys.FailPopupVariant.Id))
        {
            OnMainMenuButtonClicked = onMainMenuButtonClicked;
            OnRestartLevelClicked = onRestartLevelClicked;
        }
    }
}
