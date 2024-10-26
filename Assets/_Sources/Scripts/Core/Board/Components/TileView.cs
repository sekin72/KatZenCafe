using System;
using System.Threading;
using CerberusFramework.Core.MVC;
using CerberusFramework.Utilities.MonoBehaviourUtilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameClient.Core.Board.Components
{
    public class TileView : View
    {
        public event Action Selected;
        public event Action Released;

        [SerializeField] private GameObject _darkinator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private EasyInputManager _easyInputManager;

        private Tween _moveTween;
        private bool _selectable;

        public override void Initialize()
        {
            _easyInputManager.Selected += OnSelected;
            _easyInputManager.Released += OnReleased;

            _selectable = true;
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
            _moveTween?.Kill();

            _easyInputManager.Selected -= OnSelected;
            _easyInputManager.Released -= OnReleased;
        }

        private void OnSelected(PointerEventData pointerEventData)
        {
            if (!_selectable)
            {
                return;
            }

            Selected?.Invoke();
        }

        private void OnReleased(PointerEventData pointerEventData)
        {
            if (!_selectable)
            {
                return;
            }

            Released?.Invoke();
        }

        public void SetSelectable(bool selectable)
        {
            _selectable = selectable;
        }

        public void SetUnderContainer(Vector3Int index, ContainerView parentView, bool forceMove)
        {
            name = $"Tile_{index.x}_{index.y}";
            transform.SetParent(parentView.transform);
            if (forceMove)
            {
                transform.localPosition = Vector3.zero;
            }

            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public async UniTask MoveViewToLocalZero(CancellationToken cancellationToken)
        {
            var reached = false;
            _moveTween?.Kill();
            _moveTween = transform.DOLocalMove(Vector3.zero, .5f).SetEase(Ease.OutBack).OnComplete(() => reached = true);

            await UniTask.WaitUntil(() => reached, cancellationToken: cancellationToken);
        }

        public void SetTileState(TileState tileState)
        {
            _darkinator.SetActive(tileState == TileState.Hidden);
            SetSelectable(tileState == TileState.Shown);
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }

    public enum TileState
    {
        Hidden,
        Shown
    }
}
