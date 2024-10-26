using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Managers.UI;
using CerberusFramework.UI.Popups;
using CerberusFramework.UI.Popups.CheckConnectionPopup;
using CerberusFramework.UI.Popups.FailPopup;
using CerberusFramework.UI.Popups.LoadingPopup;
using CerberusFramework.UI.Popups.RemoteAssetDownloadPopup;
using CerberusFramework.UI.Popups.SettingsPopup;
using CerberusFramework.UI.Popups.WinPopup;
using CFGameClient.UI.Popups.FailPopupVariant;
using CFGameClient.UI.Popups.PausePopup;
using CFGameClient.UI.Popups.WinPopupVariant;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using static TMPro.TMP_Dropdown;

namespace CFGameClient.CFDemoScene
{
    public class PopupDemoPanel : DemoPanel
    {
        [SerializeField] protected TMP_Dropdown PopupDropdown;
        protected List<CFPopupTypes> PopupTypes;
        protected PopupManager PopupManager;

        [Inject]
        public void Inject(PopupManager popupManager)
        {
            PopupManager = popupManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            PopupTypes = CFPopupTypes.GetEnumerable().ToList();
            PopupTypes.RemoveAt(0);

            PopupTypes.Remove(CFPopupTypes.WinPopup);
            PopupTypes.Remove(CFPopupTypes.FailPopup);

            PopupDropdown.ClearOptions();
            foreach (var popup in PopupTypes)
            {
                PopupDropdown.options.Add(new OptionData(popup.Name));
            }

            PopupDropdown.SetValueWithoutNotify(0);
            PopupDropdown.RefreshShownValue();
        }

        public override void OnButtonClicked()
        {
            var popupType = PopupTypes[PopupDropdown.value];

            if (popupType == CFPopupTypes.CheckYourConnectionPopup)
            {
                PopupManager.Open<CheckConnectionPopup, CheckConnectionPopupView, CheckConnectionPopupData>(
                    new CheckConnectionPopupData(true, null, null), PopupShowActions.CloseAll, CancellationToken).Forget();
                return;
            }

            if (popupType == CFPopupTypes.RemoteAssetDownloadPopup)
            {
                PopupManager.Open<RemoteAssetDownloadPopup, RemoteAssetDownloadPopupView, RemoteAssetDownloadPopupData>(new RemoteAssetDownloadPopupData(), PopupShowActions.CloseAll,
                    CancellationToken).Forget();
                return;
            }

            if (popupType == CFPopupTypes.LoadingPopup)
            {
                PopupManager.Open<LoadingPopup, LoadingPopupView, LoadingPopupData>(new LoadingPopupData(), PopupShowActions.CloseAll, CancellationToken).Forget();
                return;
            }

            if (popupType == CFPopupTypes.FailPopup)
            {
                PopupManager.Open<FailPopup, FailPopupView, FailPopupData>(new FailPopupData(SceneController), PopupShowActions.CloseAll, CancellationToken).Forget();

                return;
            }

            if (popupType == CFPopupTypes.WinPopup)
            {
                PopupManager.Open<WinPopup, WinPopupView, WinPopupData>(new WinPopupData(SceneController), PopupShowActions.CloseAll, CancellationToken).Forget();
                return;
            }

            if (popupType == CFPopupTypes.SettingsPopup)
            {
                PopupManager.Open<SettingsPopup, SettingsPopupView, SettingsPopupData>(new SettingsPopupData(), PopupShowActions.CloseAll, CancellationToken).Forget();
                return;
            }

            if (popupType.Id == GamePopupKeys.FailPopupVariant.Id)
            {
                PopupManager.Open<FailPopupVariant, FailPopupVariantView, FailPopupVariantData>(new FailPopupVariantData(null, null), PopupShowActions.CloseAll, CancellationToken).Forget();

                return;
            }

            if (popupType.Id == GamePopupKeys.WinPopupVariant.Id)
            {
                PopupManager.Open<WinPopupVariant, WinPopupVariantView, WinPopupVariantData>(new WinPopupVariantData(null), PopupShowActions.CloseAll, CancellationToken).Forget();
                return;
            }

            if (popupType.Id == GamePopupKeys.PausePopup.Id)
            {
                PopupManager.Open<PausePopup, PausePopupView, PausePopupData>(new PausePopupData(null, null, null, null, null), PopupShowActions.CloseAll, CancellationToken).Forget();
            }
        }
    }
}
