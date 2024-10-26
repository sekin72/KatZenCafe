using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "LevelDesignData", menuName = "GameClient/Data/LevelDesignData", order = 3)]
    public class LevelDesignData : ScriptableObject
    {
        public List<BoardDesignData> BoardDesigns;
    }

    [Serializable]
    public class BoardDesignData
    {
        public int BoardType;
        public int Elevation;
        public int Width;
        public int Height;

        public List<Vector3Int> TilePositions;

        public int RandomTileCountMin = 6;
        public int RandomTileCountMax = 12;

        public Vector3 Position;

        public Vector3 StackDirection = Vector3.up;
        public float StackDistanceBetweenTiles = 0.05f;

        [NonSerialized]
        public int TileCount;
    }
}
