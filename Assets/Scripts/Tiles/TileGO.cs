using UnityEngine;

namespace Tiles
{
    public class TileGO : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
    
        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetTileType(TileType type)
        {
            _spriteRenderer.color = type.Color;
        }
    }
}
