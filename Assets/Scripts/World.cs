using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Managers;
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


    public World(int height, int width, WorldManager worldManager)
    {
        Height = height;
        Width = width;
        WorldManager = worldManager;
        _tiles = new Tile[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _tiles[y, x] = new Tile(x, y, this, TileType.GetRandomType());
            }
        }

        CreateMainRoad();
    }

    private void CreateMainRoad()
    {
        for (int i = 0; i < Height; i++)
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