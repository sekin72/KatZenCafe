using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient.UI
{
    public class BGAssigner : MonoBehaviour
    {
        [SerializeField] private Image _bgImage;
        [SerializeField] private List<Sprite> _bgImages;

        private void Awake()
        {
            if (_bgImage != null && _bgImages.Count > 0)
            {
                _bgImage.sprite = _bgImages[Random.Range(0, _bgImages.Count)];
            }
        }
    }
}
