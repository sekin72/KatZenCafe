using System.Threading;
using CerberusFramework.UI.Popups;
using Cysharp.Threading.Tasks;

namespace CFGameClient.UI.Popups.FailPopupVariant
{
    public class FailPopupVariant : Popup<FailPopupVariantView, FailPopupVariantData>
    {
        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            View.RestartButtonClicked += OnRestartClicked;
            View.BackToMainMenuButtonClicked += OnMainMenuButtonClicked;
        }

        public override bool Dispose()
        {
            if (base.Dispose())
            {
                return true;
            }

            View.RestartButtonClicked -= OnRestartClicked;
            View.BackToMainMenuButtonClicked -= OnMainMenuButtonClicked;

            return false;
        }

        private void OnRestartClicked()
        {
            ClosePopup();
            Data.OnRestartLevelClicked?.Invoke();
        }

        private void OnMainMenuButtonClicked()
        {
            ClosePopup();
            Data.OnMainMenuButtonClicked?.Invoke();
        }

        private void ClosePopup()
        {
            PopupManager.Close(this).Forget();
        }
    }
}
