using LazerEngine.Common.Model;
using LazerEngine.Common.Util;
using LazerEngine.Core.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model
{
    public class Screen : Common.Model.IGameComponent
    {
        private SpriteBatch ScreenSpriteBatch;
        private Camera ScreenCamera;
        public GameObjectProvider Provider { get; private set; }
        public Viewport Bounds => GameResources.CurrentViewport;
        public string Name
        {
            get; set;
        }
        public Vector2 Location { get; set; } = new Vector2();
        public Vector2 Size { get => Bounds.Bounds.Size.ToVector2(); set { } }
        public bool IsLoaded { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;

        public Screen(string name)
        {
            Name = name;
            Provider = new GameObjectProvider(this);
        }
        public void AddToScreen(EngineGameObject gameObject)
        {
            Provider.Add(gameObject);
        }
        public T CreateToScreen<T>(string ID = null) where T : EngineGameObject, new()
        {
            return Provider.Create<T>(ID);
        }
        public bool RemoveFromScreen(string ID)
        {
            return Provider.Remove(ID);
        }

        public void Load()
        {
            ScreenSpriteBatch = new SpriteBatch(GameResources.Device);
            ScreenCamera = new Camera();
            IsLoaded = true;
        }

        public void Update(GameTime gt)
        {
            foreach (var gameObject in Provider.GetAll())
                gameObject.Update(gt);
        }

        /// <summary>
        /// Begins the Draw call to the screen.
        /// </summary>
        /// <param name="batch">The default sprite batch, each screen has it's own spritebatch transformed by the Camera</param>
        public void Draw(SpriteBatch batch)
        {
            if (BackgroundColor != Color.Transparent)
                batch.Draw(GameResources.BaseTexture, Bounds.Bounds, BackgroundColor);
            ScreenSpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, ScreenCamera.Transform(GameResources.Device));
            foreach (var gameObject in Provider.GetAll())
                gameObject.Draw(ScreenSpriteBatch);
            ScreenSpriteBatch.End();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
