using System;
using CerberusFramework.UI.Components;
using CerberusFramework.UI.Popups;
using UnityEngine;

namespace CFGameClient.UI.Popups.WinPopupVariant
{
    public class WinPopupVariantView : PopupView
    {
        [SerializeField] protected CFButton BackToMainMenuButton;
        [SerializeField] protected CFButton NextLevelButton;

        public event Action BackToMainMenuButtonClicked;
        public event Action NextLevelButtonClicked;

        public override void Initialize()
        {
            base.Initialize();

            BackToMainMenuButton.onClick.AddListener(() => BackToMainMenuButtonClicked?.Invoke());
            NextLevelButton.onClick.AddListener(() => NextLevelButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            BackToMainMenuButton.onClick.RemoveListener(() => BackToMainMenuButtonClicked?.Invoke());
            NextLevelButton.onClick.RemoveListener(() => NextLevelButtonClicked?.Invoke());
        }
    }
}
