using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.Systems;
using CerberusFramework.Managers.Data;
using CerberusFramework.Managers.Loading;
using CerberusFramework.Managers.Pool;
using CerberusFramework.Managers.Sound;
using CerberusFramework.Managers.UI;
using CerberusFramework.Managers.Vibration;
using CerberusFramework.UI.Popups;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Logging;
using CFGameClient.Core.Scenes;
using CFGameClient.Data;
using CFGameClient.Managers.Data;
using CFGameClient.UI.Popups.FailPopupVariant;
using CFGameClient.UI.Popups.WinPopupVariant;
using Cysharp.Threading.Tasks;
using GameClient.GameData;
using GameClient.Managers.LevelManager;
using GameClient.UI;
using MessagePipe;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace CFGameClient.Core
{
    public class GameSession : GameSessionBase
    {
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(GameSession));
        private readonly IObjectResolver _resolver;
        private IDisposable _messageSubscription;

        private List<ITickable> _tickables;
        private List<ILateTickable> _lateTickables;

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public LockBin InputDisabled { get; private set; }

        private ProjectDataManager _projectDataManager;
        private PopupManager _popupManager;
        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;
        private AssetManager _assetManager;
        private LevelManager _levelManager;

        private List<IGameSystem> _gameSystems;
        private Dictionary<Type, IGameSystem> _gameSystemsDictionary;

        private LevelSceneController _levelSceneController;

        public GameSessionSaveStorage GameSessionSaveStorage { get; private set; }
        public GameSettings GameSettings { get; private set; }
        public BoosterPanel BoosterPanel { get; private set; }

        private bool _deactivated;
        private bool _disposed;

        public LevelDesignData LevelDesignData;

        [Inject]
        public GameSession(
            IObjectResolver resolver,
            ProjectDataManager projectDataManager,
            LoadingManager loadingManager,
            PopupManager popupManager,
            SoundManager soundManager,
            VibrationManager vibrationManager,
            AssetManager assetManager,
            LevelManager levelManager)
        {
            _resolver = resolver;
            _projectDataManager = projectDataManager;
            _popupManager = popupManager;
            _soundManager = soundManager;
            _vibrationManager = vibrationManager;
            _assetManager = assetManager;
            _levelManager = levelManager;
        }

        public override async UniTask Initialize(SceneController levelSceneController)
        {
            _disposed = false;
            _deactivated = false;

            _levelSceneController = (LevelSceneController)levelSceneController;
            BoosterPanel = _levelSceneController.BoosterPanel;
            LevelDesignData = _levelManager.SelectedLevelDesign;

            Application.targetFrameRate = 60;

            CancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = CancellationTokenSource.Token;

            _soundManager.StopAll();

            _gameSystems = ListPool<IGameSystem>.Get();
            _gameSystemsDictionary = DictionaryPool<Type, IGameSystem>.Get();

            var bagBuilder = DisposableBag.CreateBuilder();
            _messageSubscription = bagBuilder.Build();

            InputDisabled = new LockBin();
            _tickables = ListPool<ITickable>.Get();
            _lateTickables = ListPool<ILateTickable>.Get();

            GameSessionSaveStorage = _projectDataManager.ProjectSaveStorage.GameSessionSaveStorage;

            var tasks = new List<UniTask>();

            GameSettings = _assetManager.GetScriptableAsset<GameSettings>(CFPoolKeys.GameSettings);

            if (GameSettings == null)
            {
                tasks.Add(_assetManager.GetScriptableAsset<GameSettings>(CFPoolKeys.GameSettings, cancellationToken)
                 .ContinueWith((gameSettings) => GameSettings = gameSettings));
            }

            var systemsCollection = _assetManager.GetScriptableAsset<SystemsCollection>(CFPoolKeys.SystemsCollection);

            if (systemsCollection == null)
            {
                tasks.Add(_assetManager.GetScriptableAsset<SystemsCollection>(CFPoolKeys.SystemsCollection, cancellationToken)
                .ContinueWith((col) => systemsCollection = col));
            }

            if (tasks.Count > 0)
            {
                await UniTask.WhenAll(tasks);
            }

            RegisterSystems(systemsCollection);

            foreach (var system in _gameSystems)
            {
                await system.Initialize(this, cancellationToken);
            }
        }

        public void Activate()
        {
            foreach (var system in _gameSystems)
            {
                system.Activate();
            }

            RegisterTicks();
        }

        private void Deactivate()
        {
            if (_deactivated)
            {
                return;
            }

            _deactivated = true;

            if (_tickables.Count > 0)
            {
                _levelSceneController.Tick -= Tick;
            }

            if (_lateTickables.Count > 0)
            {
                _levelSceneController.LateTick -= LateTick;
            }

            ListPool<ITickable>.Release(_tickables);
            ListPool<ILateTickable>.Release(_lateTickables);

            for (var i = _gameSystems.Count - 1; i >= 0; i--)
            {
                _gameSystems[i]?.Deactivate();
            }
        }

        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Deactivate();

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;

            _disposed = true;

            _messageSubscription?.Dispose();

            for (var i = _gameSystems.Count - 1; i >= 0; i--)
            {
                _gameSystems[i]?.Dispose();
            }

            ListPool<IGameSystem>.Release(_gameSystems);
            DictionaryPool<Type, IGameSystem>.Release(_gameSystemsDictionary);
        }

        private void Tick()
        {
            for (var i = 0; i < _tickables.Count; i++)
            {
                _tickables[i].Tick();
            }
        }

        private void LateTick()
        {
            for (var i = 0; i < _lateTickables.Count; i++)
            {
                _lateTickables[i].LateTick();
            }
        }

        public void LevelFinished(bool success)
        {
            if (_deactivated)
            {
                return;
            }

            Deactivate();

            GameSessionSaveStorage.GameplayFinished = true;
            SaveGameSessionStorage();

            _soundManager.PlayOneShot(CFSoundTypes.LevelCompleted);
            _vibrationManager.Vibrate(VibrationType.Success);

            if (success)
            {
                _popupManager.Open<WinPopupVariant, WinPopupVariantView, WinPopupVariantData>(
                    new WinPopupVariantData(_levelSceneController),
                    PopupShowActions.CloseAll,
                    CancellationTokenSource.Token).Forget();
            }
            else
            {
                _popupManager.Open<FailPopupVariant, FailPopupVariantView, FailPopupVariantData>(
                    new FailPopupVariantData(
                        _levelSceneController.ReturnToMainScene,
                        _levelSceneController.RestartLevel),
                    PopupShowActions.CloseAll,
                    CancellationTokenSource.Token).Forget();
            }
        }

        private void RegisterTicks()
        {
            if (_tickables.Count > 0)
            {
                _levelSceneController.Tick += Tick;
            }

            if (_lateTickables.Count > 0)
            {
                _levelSceneController.LateTick += LateTick;
            }
        }

        private void RegisterSystems(SystemsCollection systemsCollection)
        {
            foreach (var system in systemsCollection.Systems)
            {
                _gameSystems.Add(system);
                _resolver.Inject(system);
                _gameSystemsDictionary.Add(system.RegisterType, system);

                if (system is ITickable tickable)
                {
                    _tickables.Add(tickable);
                }

                if (system is ILateTickable lateTickable)
                {
                    _lateTickables.Add(lateTickable);
                }
            }
        }

        public override T GetSystem<T>()
        {
            _gameSystemsDictionary.TryGetValue(typeof(T), out var system);
            return (T)system;
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void SaveGameSessionStorage()
        {
            _projectDataManager.ProjectSaveStorage.GameSessionSaveStorage = GameSessionSaveStorage;
            _projectDataManager.Save();
        }
    }
}
