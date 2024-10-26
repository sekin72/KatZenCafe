using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Managers.Loading;
using CerberusFramework.UI.Components;
using CFGameClient.CFDemoScene;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace CFGameClient.Core.Scenes
{
    public class CFDemoSceneController : SceneController
    {
        [SerializeField] private CFButton _backToMMButton;

        [SerializeField] private SoundDemoPanel _soundDemoPanel;
        [SerializeField] private VibrationDemoPanel _vibrationDemoPanel;
        [SerializeField] private TextTweenDemoPanel _textTweenDemoPanel;
        [SerializeField] private PopupDemoPanel _popupDemoPanel;
        [SerializeField] private ResourceDemoPanel _coinDemoPanel;
        [SerializeField] private ResourceDemoPanel _starDemoPanel;

        private CancellationTokenSource _cancellationTokenSource;
        private LoadingManager _loadingManager;

        [Inject]
        public void Inject(LoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            await base.Activate(cancellationToken);

            _backToMMButton.onClick.AddListener(ReturnToMainScene);

            _cancellationTokenSource = new CancellationTokenSource();

            _soundDemoPanel.Initialize(this, _cancellationTokenSource.Token);
            _vibrationDemoPanel.Initialize(this, _cancellationTokenSource.Token);
            _textTweenDemoPanel.Initialize(this, _cancellationTokenSource.Token);
            _popupDemoPanel.Initialize(this, _cancellationTokenSource.Token);
            _coinDemoPanel.Initialize(this, _cancellationTokenSource.Token);
            _starDemoPanel.Initialize(this, _cancellationTokenSource.Token);
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _backToMMButton.onClick.RemoveAllListeners();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _soundDemoPanel.Dispose();
            _vibrationDemoPanel.Dispose();
            _textTweenDemoPanel.Dispose();
            _popupDemoPanel.Dispose();
            _coinDemoPanel.Dispose();
            _starDemoPanel.Dispose();

            return base.Deactivate(cancellationToken);
        }

        public void ReturnToMainScene()
        {
            _loadingManager.LoadMainScene().Forget();
        }
    }
}
