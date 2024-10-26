using CerberusFramework.UI.TextTweens;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace CFGameClient.CFDemoScene
{
    public class TextTweenDemoPanel : DemoPanel
    {
        private TextTweenManager _textTweenManager;

        [Inject]
        public void Inject(TextTweenManager textTweenManager)
        {
            _textTweenManager = textTweenManager;
        }

        public override void OnButtonClicked()
        {
            _textTweenManager.ShowText("TextTween Demo!", Button.transform, Vector3.zero, CancellationToken).Forget();
        }
    }
}
