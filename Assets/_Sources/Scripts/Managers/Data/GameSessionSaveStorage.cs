using CerberusFramework.Managers.Data.Storages;

namespace CFGameClient.Managers.Data
{
    public class GameSessionSaveStorage : IStorage
    {
        public bool GameplayFinished = false;
        public int LevelRandomSeed;
    }
}
