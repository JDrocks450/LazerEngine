using LazerEngine.Common.Provider;
using LazerEngine.Content.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Content.Provider
{    
    public class TerrainProvider : IProvider
    {
        private Dictionary<Point, TerrainTile> _tiles = new Dictionary<Point, TerrainTile>();
        private Dictionary<string, (float high, float low)[]> _heightDataCache = new Dictionary<string, (float,float)[]>();
        public ProviderManager Parent { get; set; }

        public TerrainProvider(ProviderManager parent)
        {
            Parent = parent;
        }

        public TerrainTile GetTile(Point tilePos)
        {
            if (_tiles.TryGetValue(tilePos, out var value))
                return value;
            return null;
        }

        public void SetTile(Point TilePos, string tileTexturePath)
        {
            var content = ProviderManager.GetRoot().Get<Content.LazerContentManager>();
            var texture = content.GetContent<Texture2D>(tileTexturePath);
            SetTile(new TerrainTile(TilePos, tileTexturePath, texture));
        }

        public void SetTile(TerrainTile value)
        {
            InternalizeTexture(value.TextureAssetpath, ref value.TextureData);
            if (!_tiles.ContainsKey(value.TilePosition))
                _tiles.Add(value.TilePosition, value);
            else
                _tiles[value.TilePosition] = value;
        }

        private (float high, float low)[] InternalizeTexture(string texname, ref Texture2D texture)
        {
            if (_heightDataCache.TryGetValue(texname, out var value))
                return value;
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            var heightmap = new (float, float)[texture.Width];
            for (int x = 0; x < texture.Width; x++)
            {
                bool highDefined = false;
                heightmap[x] = (-1, -1);
                for (int y = 0; y < texture.Height; y++)
                {
                    var color = data[(y * texture.Width) + x];
                    if (color.A == 0) // transparent
                        continue;
                    if (!highDefined) {
                        heightmap[x] = (y, -1);
                        highDefined = true;
                        continue;
                    }
                    heightmap[x] = (heightmap[x].Item1, y);
                }
                if (heightmap[x].Item1 == -1)
                    heightmap[x].Item1 = 0;
                else if (heightmap[x].Item2 == -1)
                    heightmap[x].Item2 = TerrainTile.TILE_SIZE;
            }
            _heightDataCache.Add(texname, heightmap);
            Console.WriteLine($"TerrainProvider: Cached HeightData for {texname}, {heightmap.Length} heights");
            Console.WriteLine(string.Join(",", heightmap));
            return heightmap;
        }
    }
}
