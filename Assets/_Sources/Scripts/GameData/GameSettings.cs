using UnityEngine;

namespace CFGameClient.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "CerberusFramework/Data/GameSettings", order = 3)]
    public class GameSettings : ScriptableObject
    {
        public Vector2 TileSize = new(0.58f, 0.96f);

        public int MatchCount = 3;
        public int DeckSize = 7;
        public int MaxSpriteCount = 7;
    }
}
