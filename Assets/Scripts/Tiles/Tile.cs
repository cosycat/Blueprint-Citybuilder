using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Tiles
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }
        public World CorrespondingWorld { get; }
        public TileType Type { get; }
    
        public Structure CurrentStructure { get; internal set; }
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
            var structureType = StructureManager.Instance.GetStructureTypeForName(name);
            BuildStructureOnTile(structureType);
        }

        public void BuildStructureOnTile(StructureType structureType)
        {
            Type.BuildMethod(this, structureType);
        }

        public override string ToString()
        {
            return $"Tile ({X},{Y})";
        }
    }
    
}
