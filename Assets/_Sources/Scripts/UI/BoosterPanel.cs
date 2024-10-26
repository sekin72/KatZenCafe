using System;
using CerberusFramework.UI.Components;
using UnityEngine;

namespace GameClient.UI
{
    public class BoosterPanel : MonoBehaviour
    {
        [SerializeField] private CFButton _undoButton;
        [SerializeField] private CFButton _extraSpaceButton;
        [SerializeField] private CFButton _shuffleButton;

        public event Action UndoButtonClicked;
        public event Action ExtraSpaceButtonClicked;
        public event Action ShuffleButtonClicked;

        public void Initialize()
        {
            _undoButton.onClick.AddListener(OnUndoButtonClicked);
            _extraSpaceButton.onClick.AddListener(OnExtraSpaceButtonClicked);
            _shuffleButton.onClick.AddListener(OnShuffleButtonClicked);
        }

        public void Dispose()
        {
            _undoButton.onClick.RemoveAllListeners();
            _extraSpaceButton.onClick.RemoveAllListeners();
            _shuffleButton.onClick.RemoveAllListeners();
        }

        private void OnUndoButtonClicked()
        {
            UndoButtonClicked?.Invoke();
        }

        private void OnExtraSpaceButtonClicked()
        {
            ExtraSpaceButtonClicked?.Invoke();
        }

        private void OnShuffleButtonClicked()
        {
            ShuffleButtonClicked?.Invoke();
        }
    }
}
