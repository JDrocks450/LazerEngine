using LazerEngine.Common.Provider;
using LazerEngine.Common.Util;
using LazerEngine.Content;
using LazerEngine.Core.Model;
using LazerEngine.Core.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LazerEngine
{
    public enum DrawCallType
    {
        Text, Texture
    }
    public class LazerGameCore : Game
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected ProviderManager ProviderManager;
        protected LazerContentManager ContentManager;
        protected ScreenProvider ScreenProvider;
        protected GameObjectProvider ObjectProvider;
        protected CollisionProvider CollisionProvider;
        private static List<(DrawCallType type, Point location, object parameter)> _debugDrawCalls = new List<(DrawCallType, Point, object)>();

        public LazerGameCore(string ContentRoot = "Content", int GWidth = 1024, int GHeight = 768)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = ContentRoot;
            graphics.PreferredBackBufferWidth = GWidth;
            graphics.PreferredBackBufferHeight = GHeight;
            //graphics.SynchronizeWithVerticalRetrace = false;
        }
       
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameResources.Init(GraphicsDevice);
            ProviderManager = new ProviderManager("client");
            ContentManager = ProviderManager.Register(new LazerEngine.Content.LazerContentManager(Content));
            CollisionProvider = ProviderManager.Register(new CollisionProvider());
            ScreenProvider = ProviderManager.Register(new ScreenProvider());                
            //ScreenProvider.Add(new Screen("gameplay")
            //{
             //   BackgroundColor = Color.DeepSkyBlue
            //});
            base.Initialize();
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
            GraphicsDevice.Clear(Color.Blue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, null);
            foreach (var screen in ScreenProvider.GetAll())
            {
                screen.preDraw();
                screen.Draw(spriteBatch);                
            }
            foreach(var call in _debugDrawCalls.ToArray())
                switch (call.type)
                {
                    case DrawCallType.Text:
                        spriteBatch.DrawString(ContentManager.GetContent<SpriteFont>("Fonts/Font"), (string)call.parameter, call.location.ToVector2(), Color.White);
                        break;
                    case DrawCallType.Texture:
                        spriteBatch.Draw((Texture2D)call.parameter, 
                            new Rectangle(call.location, 
                            new Point(((Texture2D)call.parameter).Width, ((Texture2D)call.parameter).Height)), 
                            Color.White);
                        break;
                }
            spriteBatch.End();
            foreach (var screen in ScreenProvider.GetAll())
                screen.postDraw();
            _debugDrawCalls.Clear();
            base.Draw(gameTime);   
        }
        public static void DrawDebugText(string message, Point location = default)
        {
            if (location == default)
                location = new Point(10, 10);
            _debugDrawCalls.Add((DrawCallType.Text, location, message));
        }
    }
}
