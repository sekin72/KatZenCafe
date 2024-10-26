using CerberusFramework.Core.MVC;
using UnityEngine;

namespace GameClient.Core.Board.Components
{
    public class ContainerView : View
    {
        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
        }

        public void SetParent(Transform parent, Vector3Int index, Vector3 position)
        {
            gameObject.name = $"Container_{index.x}_{index.y}";
            transform.SetParent(parent);
            transform.localPosition = position;
        }
    }
}
