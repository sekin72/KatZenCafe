using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Managers.Loading;
using CerberusFramework.Managers.Sound;
using CerberusFramework.Managers.UI;
using CerberusFramework.UI.Components;
using CerberusFramework.UI.Popups;
using CerberusFramework.UI.Popups.SettingsPopup;
using CFGameClient.Managers.Data;
using Cysharp.Threading.Tasks;
using GameClient.Managers.LevelManager;
using TMPro;
using UnityEngine;
using VContainer;

namespace CFGameClient.Core.Scenes
{
    public class MainSceneController : SceneController
    {
        protected PopupManager PopupManager;
        private LoadingManager _loadingManager;
        private SoundManager _soundManager;
        private ProjectDataManager _projectDataManager;
        private LevelManager _levelManager;

        [SerializeField] private CFButton _settingsButton;

        [SerializeField] private TMP_Dropdown _levelsDropdown;
        [SerializeField] private CFButton _newGameButton;
        [SerializeField] private CFButton _randomGameButton;
        [SerializeField] private CFButton _cfDemoButton;

        [Inject]
        public void Inject(
            PopupManager popupManager,
            LoadingManager loadingManager,
            SoundManager soundManager,
            ProjectDataManager projectDataManager,
            LevelManager levelManager)
        {
            PopupManager = popupManager;
            _loadingManager = loadingManager;
            _soundManager = soundManager;
            _projectDataManager = projectDataManager;
            _levelManager = levelManager;
        }

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            await base.Activate(cancellationToken);

            _newGameButton.onClick.AddListener(OnNewGameButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _cfDemoButton.onClick.AddListener(OnCFDemoButtonClick);
            _levelsDropdown.onValueChanged.AddListener(OnSelectedLevelChanged);
            _randomGameButton.onClick.AddListener(OnRandomGameButtonClick);

            _levelsDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (var i = 0; i < _levelManager.LevelCollection.Levels.Count; i++)
            {
                _levelsDropdown.options.Add(new TMP_Dropdown.OptionData(_levelManager.LevelCollection.Levels[i].name));
            }

            RefreshLevelsDropdown(0);
            _levelManager.SetSelectedLevelDesign(0);
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _soundManager.StopAll();

            _newGameButton.onClick.RemoveListener(OnNewGameButtonClick);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            _cfDemoButton.onClick.RemoveListener(OnCFDemoButtonClick);
            _levelsDropdown.onValueChanged.RemoveListener(OnSelectedLevelChanged);
            _randomGameButton.onClick.RemoveListener(OnRandomGameButtonClick);

            return base.Deactivate(cancellationToken);
        }

        private async void OnNewGameButtonClick()
        {
            _projectDataManager.ProjectSaveStorage.GameSessionSaveStorage = new GameSessionSaveStorage
            {
                GameplayFinished = false,
                LevelRandomSeed = Mathf.Abs((int)DateTime.Now.Ticks)
            };

            await _loadingManager.LoadLevelScene();
        }

        private async void OnCFDemoButtonClick()
        {
            await _loadingManager.LoadCFDemoScene();
        }

        private void OnSettingsButtonClick()
        {
            PopupManager.Open<SettingsPopup, SettingsPopupView, SettingsPopupData>(new SettingsPopupData(), PopupShowActions.CloseAll, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async void OnRandomGameButtonClick()
        {
            _levelManager.SetSelectedLevelDesign(-1);
            await _loadingManager.LoadLevelScene();
        }

        public override void SceneVisible()
        {
            base.SceneVisible();

            _soundManager.StopAll();
            _soundManager.PlayOneShot(CFSoundTypes.FromId(GameSoundKeys.MainTheme.Id), playInLoop: true);
        }

        private void RefreshLevelsDropdown(int index)
        {
            _levelsDropdown.SetValueWithoutNotify(index);
            _levelsDropdown.value = index;

            _levelsDropdown.RefreshShownValue();
        }

        private void OnSelectedLevelChanged(int index)
        {
            _levelManager.SetSelectedLevelDesign(index);

            RefreshLevelsDropdown(index);
        }
    }
}