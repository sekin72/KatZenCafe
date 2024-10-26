using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Managers;
using CerberusFramework.Managers.Asset;
using Cysharp.Threading.Tasks;
using GameClient.GameData;
using VContainer;

namespace GameClient.Managers.LevelManager
{
    public class LevelManager : Manager
    {
        private AddressableManager _addressableManager;
        public override bool IsCore => true;

        public LevelCollection LevelCollection { get; private set; }
        public LevelDesignData SelectedLevelDesign { get; private set; }

        [Inject]
        private void Inject(AddressableManager addressableManager)
        {
            _addressableManager = addressableManager;
        }

        protected override List<IManager> GetDependencies()
        {
            return new List<IManager>
            {
                _addressableManager
            };
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            LevelCollection = await _addressableManager.LoadAssetAsync<LevelCollection>("LevelCollection");
            SelectedLevelDesign = LevelCollection.Levels[0];
            SetReady();
        }

        public void SetSelectedLevelDesign(int index)
        {
            SelectedLevelDesign = index == -1 ? LevelCollection.RandomLevelDesign : LevelCollection.Levels[index];
        }
    }
}
