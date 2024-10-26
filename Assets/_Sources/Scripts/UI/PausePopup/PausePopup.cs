using System.Threading;
using CerberusFramework.Managers.Sound;
using CerberusFramework.Managers.Vibration;
using CerberusFramework.UI.Popups;
using Cysharp.Threading.Tasks;
using VContainer;

namespace CFGameClient.UI.Popups.PausePopup
{
    public class PausePopup : Popup<PausePopupView, PausePopupData>
    {
        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            View.CloseButtonClicked += OnCloseClicked;
            View.SettingsButtonClicked += OnSettingsClicked;
            View.SaveButtonClicked += OnSaveClicked;
            View.RestartButtonClicked += OnRestartClicked;
            View.MMButtonClicked += OnMMClicked;
        }

        public override bool Dispose()
        {
            if (base.Dispose())
            {
                return true;
            }

            View.CloseButtonClicked -= OnCloseClicked;
            View.SettingsButtonClicked -= OnSettingsClicked;
            View.SaveButtonClicked -= OnSaveClicked;
            View.RestartButtonClicked -= OnRestartClicked;
            View.MMButtonClicked -= OnMMClicked;

            return false;
        }

        private void OnMMClicked()
        {
            ClosePopup();
            Data.OnMMButtonClicked?.Invoke();
        }

        private void OnSaveClicked()
        {
            Data.OnSaveButtonClicked?.Invoke();
            ClosePopup();
        }

        private void OnRestartClicked()
        {
            ClosePopup();
            Data.OnRestartButtonClicked?.Invoke();
        }

        private void OnSettingsClicked()
        {
            Data.OnSettingsButtonClicked?.Invoke();
        }

        private void OnCloseClicked()
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            PopupManager.Close(this).Forget();
            Data.OnClosed?.Invoke();
        }

        protected override void OnTapOutside()
        {
            OnCloseClicked();
            base.OnTapOutside();
        }
    }
}
