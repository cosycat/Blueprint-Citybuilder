using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Managers;
using Tiles;
using UnityEngine;

public class World
{
    public WorldManager WorldManager { get; }

    /// <summary>
    /// All the tiles of this world.
    /// First the y axis (height), then the x axis (width).
    /// </summary>
    private readonly Tile[,] _tiles;

    public int Height { get; }
    public int Width { get; }

    public BoarderRoadTile IncomingTile { get; }
    public BoarderRoadTile OutgoingTile { get; }


    public World(int height, int width, WorldManager worldManager)
    {
        Height = height;
        Width = width;
        WorldManager = worldManager;
        _tiles = new Tile[height, width];
        var incomingTileLocation = new Vector2Int(7, 0);
        var outgoingTileLocation = new Vector2Int(7, height - 1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (incomingTileLocation.x == x && incomingTileLocation.y == y)
                {
                    var incomingTile = new BoarderRoadTile(x, y, this);
                    _tiles[y, x] = incomingTile;
                    IncomingTile = incomingTile;
                }
                if (outgoingTileLocation.x == x && outgoingTileLocation.y == y)
                {
                    var outgoingTile = new BoarderRoadTile(x, y, this);
                    _tiles[y, x] = outgoingTile;
                    OutgoingTile = outgoingTile;
                }
                _tiles[y, x] = new Tile(x, y, this, TileType.GetRandomType());
            }
        }

        CreateMainRoad();
    }

    private void CreateMainRoad()
    {
        for (int i = 1; i < Height - 1; i++)
        {
            _tiles[i, 7].BuildStructureOnTile("Road");
        }
    }

    [CanBeNull]
    public Tile GetTileAt(Vector2Int coordinates)
    {
        return GetTileAt(coordinates.x, coordinates.y);
    }

    [CanBeNull]
    public Tile GetTileAt(int x, int y)
    {
        try
        {
            return _tiles[y, x];
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}