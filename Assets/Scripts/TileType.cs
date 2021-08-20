using UnityEngine;
using Random = System.Random;

public class TileType
{
    public static readonly TileType _grassTile = new TileType(Color.green);
    public static readonly TileType _dirtTile = new TileType(Color.gray);
    
    private static Random _random = new Random();

        
    public readonly Color _color;

    private TileType(Color color)
    {
        _color = color;
    }

    /// <summary>
    /// Just for testing.
    /// </summary>
    /// <returns>A tile type</returns>
    public static TileType GetRandomType()
    {
        return _grassTile;
        // return _random.Next(0, 2) > 0 ? _grassTile : _dirtTile;
    }
        
}