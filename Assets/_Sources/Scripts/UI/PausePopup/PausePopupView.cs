using System;
using CerberusFramework.UI.Components;
using CerberusFramework.UI.Popups;
using UnityEngine;

namespace CFGameClient.UI.Popups.PausePopup
{
    public class PausePopupView : PopupView
    {
        [SerializeField] protected CFButton CloseButton;
        [SerializeField] protected CFButton SettingsButton;
        [SerializeField] protected CFButton RestartButton;
        [SerializeField] protected CFButton SaveButton;
        [SerializeField] protected CFButton MMButton;

        public event Action SettingsButtonClicked;
        public event Action CloseButtonClicked;
        public event Action RestartButtonClicked;
        public event Action SaveButtonClicked;
        public event Action MMButtonClicked;

        public override void Initialize()
        {
            base.Initialize();

            CloseButton.onClick.AddListener(OnCloseButtonClicked);
            SettingsButton.onClick.AddListener(() => SettingsButtonClicked?.Invoke());
            SaveButton.onClick.AddListener(() => SaveButtonClicked?.Invoke());
            RestartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
            MMButton.onClick.AddListener(() => MMButtonClicked?.Invoke());
        }

        private void OnCloseButtonClicked()
        {
            CloseButtonClicked?.Invoke();
        }

        public override void Dispose()
        {
            SettingsButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
            MMButton.onClick.RemoveAllListeners();
            SaveButton.onClick.RemoveAllListeners();
            RestartButton.onClick.RemoveAllListeners();
        }
    }
}
