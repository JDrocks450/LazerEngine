using LazerEngine.Common.Provider;
using LazerEngine.Common.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Common.Model
{
    /// <summary>
    /// Rewriting XNA GameComponent because it's not a good idea to use it
    /// </summary>
    public abstract class LazerGameComponent : IGameComponent
    {
        public string ID
        {
            get; set;
        }

        public Texture2D Texture
        {
            get;set;
        }
        public Vector2 Location;

        public Vector2 Size
        {
            get; set;
        }

        public Rectangle Hitbox
        {
            get => new Rectangle(Location.ToPoint(), (Size * new Vector2(Scale)).ToPoint());
            set
            {
                Location = value.Location.ToVector2();
                Size = value.Size.ToVector2();
            }
        }

        private Rectangle _textureDestination;
        public Rectangle TextureDestination
        {
            get
            {
                if (_textureDestination == default)
                    return new Rectangle(Hitbox.Location, Hitbox.Size);
                return _textureDestination;
            }
            set => _textureDestination = value;
        }

        public SpriteEffects SpriteFlipSetting { get; set; } = SpriteEffects.None;
        public Rectangle? TextureSource
        {
            get; set;
        } = null;

        public Color Color
        {
            get; set;
        } = Color.Silver;

        public virtual Color Ambience
        {
            get; set;
        } = Color.Transparent;

        public virtual float AmbientIntensity
        {
            get; set;
        } = 0f;

        /// <summary>
        /// The origin of the sprite. Zero to One, 1 = Sprite Width/Height, 0 = 0
        /// </summary>
        public Vector2 Origin
        {
            get; set;
        }

        public float Scale
        {
            get; set;
        } = 2f;

        public float Rotation
        {
            get; set;
        }

        public bool IsLoaded { get; set; }

        public LazerGameComponent(string ID)
        {
            if (ID == null)
                ID = GetID();
            this.ID = ID;
        }

        public abstract void Load();

        public abstract void Update(GameTime gt);

        public virtual void Draw(SpriteBatch batch)
        {
            var origin = Origin * Size;
            batch.Draw(Texture,
                new Rectangle(TextureDestination.Location, (TextureDestination.Size.ToVector2()).ToPoint()),
                TextureSource,
                Color.Lerp(Color, Ambience, AmbientIntensity),
                Rotation, origin, SpriteFlipSetting, 0);
        }

        public static string GetID(char prefix = 'O', IEnumerable<LazerGameComponent> scope = null)
        {
            string getID()
            {
                return prefix + GameResources.Rand.Next(10000, 99999).ToString();
            }
            if (scope == null)
                return getID();
            var id = getID();
            bool ok = false;
            while (!ok)
            {
                ok = true;
                foreach (var item in scope)
                    if (item.ID == id)
                    {
                        ok = false;
                        break;
                    }
                if (!ok)
                    id = getID();
            }
            return id;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LazerGameComponent()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
