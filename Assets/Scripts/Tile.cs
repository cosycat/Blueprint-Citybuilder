using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class Tile
{
    public int X { get; }
    public int Y { get; }
    public World CorrespondingWorld { get; }
    public TileType Type { get; }
    
    public Structure CurrentStructure { get; private set; }
    public bool HasStructure => CurrentStructure != null;

    public TileGO CorrespondingGameObject { get; private set; }

    public Tile(int x, int y, World world, TileType type)
    {
        X = x;
        Y = y;
        CorrespondingWorld = world;
        Type = type;
        CreateGameWorldTile();
    }

    private void CreateGameWorldTile()
    {
        CorrespondingGameObject = CorrespondingWorld.WorldManager.CreateGameWorldTile(X, Y, this);
        CorrespondingGameObject.SetTileType(Type);
        // TODO adjust the TileGO to look according to this tile's data.
    }

    public void BuildStructureOnTile(string name)
    {
        if (CurrentStructure != null)
            throw new Exception($"Tile {this.ToString()} already has a structure on it!");
        
        var newStructure = StructureManager.Instance.CreateNewStructureForName(name);
        newStructure.transform.position = CorrespondingGameObject.transform.position;
        CurrentStructure = newStructure;
    }

    public override string ToString()
    {
        return $"Tile ({X},{Y})";
    }
}
