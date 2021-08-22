using JetBrains.Annotations;
using UnityEngine;

namespace Managers
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }
        [SerializeField] private Vector2Int worldDimension = new Vector2Int(10, 10);


        private World _world;

        [SerializeField] private TileGO tilePrefab;
        public TileGO TilePrefab => tilePrefab;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _world = new World(worldDimension.x, worldDimension.y, this);
        }

        public TileGO CreateGameWorldTile(int x, int y, Tile tile)
        {
            TileGO tileGO = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
            tileGO.transform.SetParent(transform);
            tileGO.name = $"Tile ({x}, {y})";
            return tileGO;
        }

        public Vector2Int GetTileCoordinatesForRealWorldCoordinates(Vector2 coordinates)
        {
            return Vector2Int.RoundToInt(coordinates);
        }

        public Vector3 GetWorldCoordinatesForTileCoordinates(Vector2Int tileCoord)
        {
            return new Vector3(tileCoord.x, tileCoord.y);
        }

        public bool IsTileInWorld(Vector2Int tileCoord)
        {
            return tileCoord.x >= 0
                   && tileCoord.y >= 0
                   && tileCoord.x < worldDimension.x
                   && tileCoord.y < worldDimension.y;
        }

        [CanBeNull]
        public Tile GetTileForRealWorldCoordinates(Vector2 v)
        {
            return GetTileForRealWorldCoordinates(v.x, v.y);
        }

        [CanBeNull]
        public Tile GetTileForRealWorldCoordinates(float x, float y)
        {
            return _world.GetTileAt(GetTileCoordinatesForRealWorldCoordinates(new Vector2(x, y)));
        }

        [CanBeNull]
        public Tile GetTileAt(Vector2Int v)
        {
            return _world.GetTileAt(v);
        }
    }
}