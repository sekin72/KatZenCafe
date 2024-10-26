using System;
using CerberusFramework.UI.Components;
using CerberusFramework.UI.Popups;
using UnityEngine;

namespace CFGameClient.UI.Popups.FailPopupVariant
{
    public class FailPopupVariantView : PopupView
    {
        [SerializeField] protected CFButton RestartButton;
        [SerializeField] protected CFButton BackToMainMenuButton;

        public event Action RestartButtonClicked;
        public event Action BackToMainMenuButtonClicked;

        public override void Initialize()
        {
            base.Initialize();

            BackToMainMenuButton.onClick.AddListener(() => BackToMainMenuButtonClicked?.Invoke());
            RestartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            BackToMainMenuButton.onClick.RemoveListener(() => BackToMainMenuButtonClicked?.Invoke());
            RestartButton.onClick.RemoveListener(() => RestartButtonClicked?.Invoke());
        }
    }
}