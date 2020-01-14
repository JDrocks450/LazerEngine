using LazerEngine.Common.Provider;
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
        public LazerContentManager(ContentManager manager) : base("content")
        {
            Manager = manager;
        }

        public T GetContent<T>(string key)
        {
            return Manager.Load<T>(key);
        }
    }
}
