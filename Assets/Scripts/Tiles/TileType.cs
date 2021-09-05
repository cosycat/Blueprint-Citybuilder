using System;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using static Tiles.BuildMethods;
using Random = System.Random;

namespace Tiles
{
    public class TileType
    {
        public static readonly TileType GrassTile = new TileType(Color.green, StandardStructureBuildMethod);
        public static readonly TileType DirtTile = new TileType(Color.gray, StandardStructureBuildMethod);

        public static readonly TileType SpecialTile = new TileType(Color.black, ForbiddenStructureBuildMethod);
        public static readonly TileType BoarderRoadTile = new TileType(SpecialTile, Color.green, ForbiddenStructureBuildMethod);

        private static Random _random = new Random();

        public readonly Color Color;
        internal readonly StructureBuildMethod BuildMethod;

        private TileType(Color color, StructureBuildMethod buildMethod)
        {
            this.Color = color;
            this.BuildMethod = buildMethod;
        }

        /// <summary>
        /// Creates a child Type, that uses the given parent's values for every value which was not supplied (=is null).
        /// 
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="color"></param>
        /// <param name="buildMethod"></param>
        /// <exception cref="ArgumentNullException">If the parentType is null.</exception>
        private TileType([NotNull] TileType parentType, Color? color, StructureBuildMethod buildMethod) :
            this(color ?? parentType.Color, buildMethod ?? parentType.BuildMethod)
        {
            if (parentType == null) throw new ArgumentNullException(nameof(parentType));
        }

        /// <summary>
        /// Just for testing.
        /// </summary>
        /// <returns>A tile type</returns>
        public static TileType GetRandomType()
        {
            return GrassTile;
            // return _random.Next(0, 2) > 0 ? _grassTile : _dirtTile;
        }
    }

    internal static class BuildMethods
    {
        internal delegate void StructureBuildMethod(Tile tile, StructureType structureType);

        internal static readonly StructureBuildMethod StandardStructureBuildMethod =
            (tile, structureType) =>
            {
                if (tile.CurrentStructure != null)
                    throw new Exception($"Tile {tile} already has a structure on it!");

                var newStructure = StructureManager.Instance.CreateNewStructureForStructureType(structureType);
                newStructure.transform.position = tile.CorrespondingGameObject.transform.position;
                tile.CurrentStructure = newStructure;
            };

        internal static readonly StructureBuildMethod ForbiddenStructureBuildMethod =
            (tile, structureType) => { Debug.LogWarning("Can't build on a Special Tile!"); };
    }
}