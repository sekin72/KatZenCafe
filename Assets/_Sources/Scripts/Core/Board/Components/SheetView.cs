using CerberusFramework.Core.MVC;
using UnityEngine;

namespace GameClient.Core.Board.Components
{
    public class SheetView : View
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

        public void SetParent(Transform parent, int index, Vector3 localPosition)
        {
            gameObject.name = $"Sheet_{index}";
            transform.SetParent(parent);
            transform.localPosition = localPosition;
        }
    }
}
