using System;
using System.Threading;
using CerberusFramework.Managers.Data;
using CerberusFramework.Managers.Data.Syncers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CFGameClient.Managers.Data
{
    public class ProjectDataManager : DataManager
    {
        public ProjectSaveStorage ProjectSaveStorage
        {
            get
            {
                return _saveStorage;
            }
        }

        private ILocalSyncer<ProjectSaveStorage> _localSyncer;
        private ProjectSaveStorage _saveStorage;

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            SaveKey = "ProjectSave";

            _localSyncer = new LocalStorageSyncer<ProjectSaveStorage>(SaveKey, PlayerPrefs.GetInt("PlayerID").ToString());

            _saveStorage = await _localSyncer.Load(disposeToken);

            StartAutoSavingJob(disposeToken).Forget();
            SetReady();
        }

        protected override void SaveLocal()
        {
            _localSyncer.Save(_saveStorage);
        }

        public override T Load<T>()
        {
            return !IsReady() ? throw new InvalidOperationException("Trying to load data before ProjectDataManager is ready") : _saveStorage.Get<T>();
        }

        public override void Save<T>(T data)
        {
            if (!IsReady())
            {
                throw new InvalidOperationException("Trying to save data before ProjectDataManager is ready");
            }

            _saveStorage.Set(data);
            IsSaveDirty = true;
        }
    }
}
