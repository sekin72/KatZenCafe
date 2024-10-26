using System;
using System.Threading;
using CerberusFramework.Core.Scenes;
using CerberusFramework.UI.Components;
using UnityEngine;

namespace CFGameClient.CFDemoScene
{
    public abstract class DemoPanel : MonoBehaviour, IDisposable
    {
        public CFButton Button;

        protected SceneController SceneController;
        protected CancellationToken CancellationToken;

        public virtual void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            SceneController = sceneController;
            CancellationToken = cancellationToken;
            Button.onClick.AddListener(OnButtonClicked);
        }

        public virtual void Dispose()
        {
            Button.onClick.RemoveAllListeners();
        }

        public abstract void OnButtonClicked();
    }
}
