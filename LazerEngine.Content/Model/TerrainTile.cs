using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Content.Model
{
    public class TerrainTile
    {
        public const int TILE_SIZE = 100;

        public readonly Point TileSize;
        public Point TilePosition;
        public string TextureAssetpath;
        public Texture2D TextureData;
        public (float high, float low)[] CollisionData;

        public TerrainTile(Point TilePosition, string TextureAssetName, Texture2D Texture)
        {
            this.TilePosition = TilePosition;
            TextureData = Texture;
            TextureAssetpath = TextureAssetName;
            TileSize = new Point(TILE_SIZE, TILE_SIZE);
        }
    }
}
