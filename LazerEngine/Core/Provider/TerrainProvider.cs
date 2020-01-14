using LazerEngine.Common.Provider;
using LazerEngine.Core.Model.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazerEngine.Core.Provider
{
    public class TerrainProvider : IProvider
    {        
        // each layer identified first by screen name, then next by LayerIndex
        private static Dictionary<string, Dictionary<int, TerrainProvider>> _layersByScreen = new Dictionary<string, Dictionary<int, TerrainProvider>>();

        private Dictionary<Point, TerrainTile> _tiles = new Dictionary<Point, TerrainTile>();
        private Dictionary<string, (float high, float low)[]> _heightDataCache = new Dictionary<string, (float,float)[]>();

        public ProviderManager Parent { get; set; }
        public static Point TileSize
        {
            get => TerrainTile.TileSize;
            set => TerrainTile.TileSize = value;
        }        

        public TerrainProvider()
        {
            
        }

        public TerrainTile GetTile(Point tilePos)
        {
            if (_tiles.TryGetValue(tilePos, out var value))
                return value;
            return null;
        }

        public TerrainTile[] GetTilesByArea(Rectangle Area)
        {
            var array = new TerrainTile[4];
            var point = Area.Location;
            array[0] = GetTile(TerrainTile.ConvertWorldToTilePos(point));
            point = Area.Location + new Point(Area.Width,0);
            array[1] = GetTile(TerrainTile.ConvertWorldToTilePos(point));
            point = Area.Location + new Point(0,Area.Height);
            array[2] = GetTile(TerrainTile.ConvertWorldToTilePos(point));
            point = Area.Location + Area.Size;
            array[3] = GetTile(TerrainTile.ConvertWorldToTilePos(point));
            return array;
        }

        public (float high, float low)[] GetHeightRangeScaled(TerrainTile tile, int worldStartX, int worldEndX)
        {
            if (tile.CollisionData.Length == 0)
                return null;            
            var scaleX = (float)TileSize.X / tile.CollisionData.Length;
            var scaleY = (float)TileSize.Y / tile.TextureData.Height;
            int scaledStartX = (int)(scaleX * worldStartX), scaledEndX = (int)(scaleX * worldEndX);
            var heightMap = GetHeightRange(tile, scaledStartX, scaledEndX);
            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i].high *= scaleY;
                heightMap[i].low *= scaleY;
            }
            return heightMap;
        } 

        public (float high, float low)[] GetHeightRange(TerrainTile tile, int arrStartX, int arrEndX)
        {
            return tile.CollisionData.Skip(arrStartX).Take(arrEndX - arrStartX).ToArray();
        }

        public void SetTile(Point TilePos, string tileTexturePath)
        {
            var content = ProviderManager.GetRoot().Get<Content.LazerContentManager>();
            var texture = content.GetContent<Texture2D>(tileTexturePath);
            SetTile(new TerrainTile(TilePos, tileTexturePath, texture));
        }

        public void SetTile(TerrainTile value)
        {
            value.CollisionData = InternalizeTexture(value.TextureAssetpath, ref value.TextureData);
            if (!_tiles.ContainsKey(value.TilePosition))
                _tiles.Add(value.TilePosition, value);
            else
                _tiles[value.TilePosition] = value;
        }

        public static TerrainProvider GetLayer(string screenName, int layer)
        {
            if (_layersByScreen[screenName].TryGetValue(layer, out var value))
                return value;
            return null;
        }

        /// <summary>
        /// Creates a new layer at the specified LayerIndex
        /// </summary>
        /// <param name="layer">-1 Adds a Top-Most Layer, if the index is already used, this will return false.</param>
        /// <param name="provider">The created provider for the Layer specifed</param>
        public static bool CreateLayer(string screenName, out TerrainProvider provider, int layer = -1, Point TileSize = default)
        {
            if (!_layersByScreen.ContainsKey(screenName))
                _layersByScreen.Add(screenName, new Dictionary<int, TerrainProvider>());
            if (!_layersByScreen[screenName].ContainsKey(layer))
            {
                _layersByScreen[screenName].Add(layer, new TerrainProvider());
                provider = _layersByScreen[screenName][layer];
                return true;
            }
            provider = null;
            return false;
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
                    heightmap[x].Item2 = TileSize.Y;
            }
            _heightDataCache.Add(texname, heightmap);
            Console.WriteLine($"TerrainProvider: Cached HeightData for {texname}, {heightmap.Length} heights");
            Console.WriteLine(string.Join(",", heightmap));
            return heightmap;
        }

        /// <summary>
        /// Draws all layers, in layer order
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="viewrect"></param>
        /// <param name="ambient"></param>
        public static void DrawLayers(string screenName, SpriteBatch batch, Rectangle viewrect, Color ambient)
        {
            foreach (var layerprovider in _layersByScreen[screenName])
                layerprovider.Value.Draw(batch, viewrect, ambient);
        }

        /// <summary>
        /// Draws the selected layers, in layer order.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        public static void DrawLayers(string screenName, int first, int last, SpriteBatch batch, Rectangle viewrect, Color ambient)
        {
            foreach (var layerprovider in _layersByScreen[screenName].Skip(first).Take(last - first))
                layerprovider.Value.Draw(batch, viewrect, ambient);
        }

        /// <summary>
        /// A macro to draw every terrain tile in the scene and cull the rest.
        /// </summary>
        /// <param name="batch">The batch to draw to</param>
        /// <param name="Viewrect">The current screen perspective</param>
        public void Draw(SpriteBatch batch, Rectangle Viewrect, Color Ambient)
        {
            foreach (var tile in _tiles.Values)
            {
                var tileDest = tile.GetTileRectangle();
               // if (Viewrect.Intersects(tileDest))
                    batch.Draw(tile.TextureData, tileDest, Color.Lerp(Color.White, Ambient, .5f));
            }
        }
    }
}
