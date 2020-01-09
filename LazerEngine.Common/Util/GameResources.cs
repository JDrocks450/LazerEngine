using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Common.Util
{
    public static class GameResources
    {
        public static Random Rand = new Random();
        public static Viewport CurrentViewport => Device.Viewport;
        public static GraphicsDevice Device;
        public static Texture2D BaseTexture { get; private set; }

        public static void Init(GraphicsDevice device)
        {
            Device = device;
            BaseTexture = new Texture2D(device, 1, 1);
            BaseTexture.SetData(new Color[] { Color.White });
        }
    }
}
