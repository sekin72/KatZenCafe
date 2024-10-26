using System;
using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Managers.Loading;
using CerberusFramework.Managers.Pool;
using CerberusFramework.Managers.UI;
using CerberusFramework.UI.Components;
using CerberusFramework.UI.Popups;
using CerberusFramework.UI.Popups.LoadingPopup;
using CerberusFramework.UI.Popups.SettingsPopup;
using CFGameClient.UI.Popups.PausePopup;
using Cysharp.Threading.Tasks;
using GameClient.UI;
using UnityEngine;
using VContainer;

namespace CFGameClient.Core.Scenes
{
    public class LevelSceneController : SceneController
    {
        public event Action Tick;
        public event Action LateTick;

        private IObjectResolver _resolver;
        private PopupManager _popupManager;
        private LoadingManager _loadingManager;
        private PoolWarmUpManager _poolWarmUpManager;

        private GameSession _session;
        private IDisposable _messageSubscription;
        private CancellationTokenSource _oldCancellationTokenSource;
        private CancellationToken _originalCancellationToken;

        [SerializeField] private GameObject _light;
        [SerializeField] private CFButton _pauseButton;

        public BoosterPanel BoosterPanel;

        [Inject]
        public void Inject(
            IObjectResolver objectResolver,
            PopupManager popupManager,
            LoadingManager loadingManager,
            PoolWarmUpManager levelWarmUpManager)
        {
            _resolver = objectResolver;
            _popupManager = popupManager;
            _loadingManager = loadingManager;
            _poolWarmUpManager = levelWarmUpManager;
        }

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            _resolver.Inject(UIContainer);

            _originalCancellationToken = cancellationToken;

            await base.Activate(cancellationToken);

            await LoadLevel();
        }

        public override async UniTask Deactivate(CancellationToken cancellationToken)
        {
            _session?.Dispose();
            _session = null;

            _messageSubscription?.Dispose();
            _messageSubscription = null;

            _pauseButton.onClick.RemoveAllListeners();

            await base.Deactivate(cancellationToken);
        }

        public override void SceneVisible()
        {
            _light.SetActive(true);
            base.SceneVisible();
        }

        public override void SceneInvisible()
        {
            base.SceneInvisible();

            _light.SetActive(false);
            _popupManager.ClearAll(true).Forget();
        }

        private async UniTask LoadLevel()
        {
            _pauseButton.onClick.RemoveAllListeners();

            if (_oldCancellationTokenSource != null)
            {
                _oldCancellationTokenSource.Cancel();
                _oldCancellationTokenSource.Dispose();
            }

            _session?.Dispose();
            _session = null;

            await LoadLevelInternal();

            _session.Activate();
        }

        private async UniTask LoadLevelInternal()
        {
            _oldCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_originalCancellationToken);
            var linkedCancellationToken = _oldCancellationTokenSource.Token;

            if (!_poolWarmUpManager.LevelWarmUpCompleted)
            {
                await _poolWarmUpManager.StartRemainingJobsForceful(linkedCancellationToken);
            }

            _session = _resolver.Resolve<GameSession>();

            await _popupManager.Open<LoadingPopup, LoadingPopupView, LoadingPopupData>(new LoadingPopupData(), PopupShowActions.CloseAll, linkedCancellationToken);

            await _session.Initialize(this);

            _pauseButton.onClick.AddListener(() => OpenPausePopup(linkedCancellationToken));

            var popup = _popupManager.GetPopup<LoadingPopup>();
            await _popupManager.Close(popup, false);
        }

        private void OpenPausePopup(CancellationToken cancellationToken)
        {
            _session.PauseGame();
            _popupManager.Open<PausePopup, PausePopupView, PausePopupData>(new PausePopupData(null,
                    () =>
                    {
                        _popupManager.Open<SettingsPopup, SettingsPopupView, SettingsPopupData>(new SettingsPopupData(), PopupShowActions.HideAll,
                            cancellationToken).Forget();
                    }, RestartLevel, ReturnToMainScene, _session.ResumeGame), PopupShowActions.CloseAll,
                cancellationToken).Forget();
        }

        public void ReturnToMainScene()
        {
            _session.Dispose();
            _session = null;

            _loadingManager.LoadMainScene().Forget();
        }

        public void RestartLevel()
        {
            _session.Dispose();
            _session = null;

            LoadLevel().Forget();
        }

        private void Update()
        {
            Tick?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }

        private void LateUpdate()
        {
            LateTick?.Invoke();
        }
    }
}
