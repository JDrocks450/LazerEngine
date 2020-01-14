using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model.Terrain
{
    public class TerrainTile
    {
        public static Point TileSize { get; set; } = new Point(150, 150);
        public Point TilePosition;
        public string TextureAssetpath;
        public Texture2D TextureData;
        public (float high, float low)[] CollisionData = null;
        public bool HasCollision => CollisionData != null;

        public TerrainTile(Point TilePosition, string TextureAssetName, Texture2D Texture)
        {
            this.TilePosition = TilePosition;
            TextureData = Texture;
            TextureAssetpath = TextureAssetName;
        }

        public bool Contains(Point Location)
        {
            return GetTileRectangle().Contains(Location);
        }

        public Point GetWorldTileLocation()
        {
            return TilePosition * TileSize;
        }

        public static Point ConvertTileToWorldPos(Point tilePos)
        {
            return tilePos * TileSize;
        }

        public static Point ConvertWorldToTilePos(Point worldPos)
        {
            return new Point((int)(worldPos.X / TileSize.X), (int)(worldPos.Y / TileSize.Y));
        }

        public Rectangle GetTileRectangle()
        {
            return new Rectangle(GetWorldTileLocation(), TileSize);
        }
    }
}
