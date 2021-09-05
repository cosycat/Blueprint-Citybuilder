using Managers;

namespace Tiles
{
    public abstract class SpecialTile : Tile
    {
        protected SpecialTile(int x, int y, World world) : base(x, y, world, TileType.SpecialTile)
        {
            
        }
        
        public override string ToString()
        {
            return $"Special Tile ({X},{Y})";
        }
    }

    public class BoarderRoadTile : SpecialTile
    {
        public BoarderRoadTile(int x, int y, World world) : base(x, y, world)
        {
            BuildMethods.StandardStructureBuildMethod(this, StructureManager.Instance.road);
        }
    }
}