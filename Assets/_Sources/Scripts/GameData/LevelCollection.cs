using System.Collections.Generic;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "LevelCollection", menuName = "GameClient/LevelCollection", order = 3)]
    public class LevelCollection : ScriptableObject
    {
        public List<LevelDesignData> Levels;
        public LevelDesignData RandomLevelDesign;
    }
}
