using LazerEngine.Common.Provider;
using LazerEngine.Content.Provider;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Content
{
    public class LazerContentManager : ProviderManager
    {
        private ContentManager Manager;
        private TerrainProvider TerrainProvider;
        public LazerContentManager(ContentManager manager) : base("content")
        {
            Manager = manager;
            TerrainProvider = (TerrainProvider)Register(new TerrainProvider(this));
        }

        public T GetContent<T>(string key)
        {
            return Manager.Load<T>(key);
        }
    }
}
