using LazerEngine.Common.Provider;
using LazerEngine.Common.Util;
using LazerEngine.Content;
using LazerEngine.Content.Provider;
using LazerEngine.Core.Model;
using LazerEngine.Core.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine
{
    public class LazerGameCore : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ProviderManager ProviderManager;
        LazerContentManager ContentManager;
        ScreenProvider ScreenProvider;

        public LazerGameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.SynchronizeWithVerticalRetrace = false;
        }
       
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameResources.Init(GraphicsDevice);
            ProviderManager = new ProviderManager("client");
            ContentManager = (LazerContentManager)ProviderManager.Register(new LazerEngine.Content.LazerContentManager(Content));
            ScreenProvider = (ScreenProvider)ProviderManager.Register(new ScreenProvider());
            ScreenProvider.Add(new Screen("gameplay")
            {
                BackgroundColor = Color.DeepSkyBlue
            });
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            var terrain = ContentManager.Get<TerrainProvider>();
            terrain.SetTile(new Point(1, 1), "map1");
            ScreenProvider.Get("gameplay").Load();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var screen in ScreenProvider.GetAll())
                screen.Update(gameTime);            
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            foreach (var screen in ScreenProvider.GetAll())
                screen.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);   
        }
    }
}
