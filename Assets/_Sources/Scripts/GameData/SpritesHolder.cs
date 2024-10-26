using System.Collections.Generic;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "SpritesHolder", menuName = "GameClient/Data/SpritesHolder", order = 2)]
    public class SpritesHolder : ScriptableObject
    {
        public List<Sprite> Sprites;
    }
}
