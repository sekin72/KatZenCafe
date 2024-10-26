using System;
using CerberusFramework.Managers.Pool;
using CerberusFramework.UI.Popups;

namespace CFGameClient.UI.Popups.PausePopup
{
    public class PausePopupData : PopupData
    {
        public readonly Action OnSaveButtonClicked;
        public readonly Action OnSettingsButtonClicked;
        public readonly Action OnRestartButtonClicked;
        public readonly Action OnMMButtonClicked;
        public readonly Action OnClosed;

        public PausePopupData(Action onSaveButtonClicked, Action onSettingsButtonClicked, Action onRestartButtonClicked, Action mmButtonClicked, Action onClosed)
            : base(CFPoolKeys.FromId(GamePoolKeys.PausePopup.Id))
        {
            OnSaveButtonClicked = onSaveButtonClicked;
            OnSettingsButtonClicked = onSettingsButtonClicked;
            OnRestartButtonClicked = onRestartButtonClicked;
            OnMMButtonClicked = mmButtonClicked;
            OnClosed = onClosed;
        }
    }
}
