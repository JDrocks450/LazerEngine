using LazerEngine.Common.Provider;
using LazerEngine.Common.Util;
using LazerEngine.Core.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazerEngine.Core.Model
{
    public class Screen : Common.Model.IGameComponent
    {
        private bool isLoadedMsgDisplayed = false;
        protected SpriteBatch ScreenSpriteBatch;
        protected Camera ScreenCamera;
        public GameObjectProvider ObjectProvider { get; private set; }
        public Viewport Bounds => GameResources.CurrentViewport;
        public string Name
        {
            get; set;
        }
        public Vector2 Location { get; set; } = new Vector2();
        public Vector2 Size { get => Bounds.Bounds.Size.ToVector2(); set { } }
        public bool IsLoaded { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public Color AmbientColor { get; set; } = Color.White;
        public float AmbientIntensity { get; set; } = 1f;
        public float Gravity { get; set; } = -9.81f;
        /// <summary>
        /// Determines whether or not the terrain will draw automatically in a Screen object's update loop.
        /// </summary>
        public bool AutoDrawTerrain
        {
            get; set;
        } = true;

        private List<Texture2D> paralax_drawList = new List<Texture2D>();

        public Screen(string name)
        {
            Name = name;     
            ObjectProvider = ProviderManager.Root.Register(new GameObjectProvider(this));
        }
        public void AddToScreen(EngineGameObject gameObject)
        {
            ObjectProvider.Add(gameObject);
        }
        public T CreateToScreen<T>(string ID = null) where T : EngineGameObject, new()
        {
            return ObjectProvider.Create<T>(ID);
        }
        public bool RemoveFromScreen(string ID)
        {
            return ObjectProvider.Remove(ID);
        }

        public virtual void Load()
        {            
            ScreenSpriteBatch = new SpriteBatch(GameResources.Device);
            ScreenCamera = new Camera();
            IsLoaded = true;
        }

        public void AddParalaxTexture(Texture2D texture)
        {
            paralax_drawList.Add(texture);
        }

        public virtual void Update(GameTime gt)
        {
            if (!IsLoaded && !isLoadedMsgDisplayed)
            {
                Console.WriteLine("[DANGEROUS] Load has not been called on the Screen: " + Name);
                isLoadedMsgDisplayed = true;
            }
            var kboard = Keyboard.GetState();
            if (kboard.IsKeyDown(Keys.Left))
                ScreenCamera.Pos.X--;
            if (kboard.IsKeyDown(Keys.Right))
                ScreenCamera.Pos.X++;
            if (kboard.IsKeyDown(Keys.Up))
                ScreenCamera.Pos.Y--;
            if (kboard.IsKeyDown(Keys.Down))
                ScreenCamera.Pos.Y++;
            LazerGameCore.DrawDebugText("ScreenCamera Pos: " + ScreenCamera.Pos);
            foreach (var gameObject in ObjectProvider.GetAll())
                gameObject.Update(gt);
            ProviderManager.Root.Get<CollisionProvider>().UpdateCollision(this, gt);
        }

        internal void preDraw()
        {
            ScreenSpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, ScreenCamera.Transform(GameResources.Device));
        }

        internal void postDraw()
        {
            ScreenSpriteBatch.End();
        }

        /// <summary>
        /// Begins the Draw call to the screen.
        /// </summary>
        /// <param name="batch">The default sprite batch, each screen has it's own spritebatch transformed by the Camera</param>
        public virtual void Draw(SpriteBatch batch)
        {
            if (BackgroundColor != Color.Transparent)
                batch.Draw(GameResources.BaseTexture, Bounds.Bounds, BackgroundColor);
            if (!IsLoaded)
                return;
            if (paralax_drawList.Any())
            {
                int i = paralax_drawList.Count;
                foreach(var sky in paralax_drawList)
                {
                    const int paralaxScaleX = 2;
                    const float paralaxSpeed = 2f;
                    var paralaxPoint = (ScreenCamera.Pos / new Vector2(i == 0 ? 1f : -i * paralaxSpeed, 1)).ToPoint();
                    var size = Bounds.Bounds.Size * new Point(paralaxScaleX,1);
                    if (paralaxPoint.X > 0)
                    {
                        int divisible = (int)(paralaxPoint.X / (float)(size.X / paralaxScaleX)) +1;
                        paralaxPoint.X -= divisible * (size.X / paralaxScaleX);
                    }
                    else if (-paralaxPoint.X >= (size.X / paralaxScaleX))
                    {
                        int divisible = (int)(-paralaxPoint.X / (float)(size.X / paralaxScaleX));
                        paralaxPoint.X += divisible * (size.X / paralaxScaleX);
                    }
                    batch.Draw(sky,
                        new Rectangle(paralaxPoint, size),
                        new Rectangle(0,0,Bounds.Width /2, Bounds.Height/4),
                        Color.White);
                    i--;
                }
            }
            if (AutoDrawTerrain)
                TerrainProvider.DrawLayers(Name, ScreenSpriteBatch, ScreenCamera.Screen, AmbientColor);
            foreach (var gameObject in ObjectProvider.GetAll())
                gameObject.Draw(ScreenSpriteBatch);
            ProviderManager.Root.Get<CollisionProvider>().DebugDraw(ScreenSpriteBatch);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
