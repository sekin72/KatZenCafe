using CerberusFramework.Managers.Data.Storages;

namespace CFGameClient.Managers.Data
{
    public class ProjectSaveStorage : SaveStorage
    {
        public GameSessionSaveStorage GameSessionSaveStorage { get; set; }

        public ProjectSaveStorage() : base()
        {
        }

        protected override void RegisterStorages()
        {
            Storages.Add(
                typeof(GameSessionSaveStorage), new StorageProperty
                {
                    Get = () => GameSessionSaveStorage,
                    Set = data => GameSessionSaveStorage = (GameSessionSaveStorage)data
                }
            );
        }
    }
}
