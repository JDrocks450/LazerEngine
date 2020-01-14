using LazerEngine.Common.Model;
using LazerEngine.Common.Primitives;
using LazerEngine.Common.Provider;
using LazerEngine.Common.Util;
using LazerEngine.Core.Model.Terrain;
using LazerEngine.Core.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model
{
    public abstract class EngineGameObject : LazerGameComponent
    {
        public bool IsOnScreen;
        public Screen Parent;
        /// <summary>
        /// The current tile that this object is sitting in
        /// </summary>
        public TerrainTile CurrentTile
        {
            get
            {
                if (!_currTile.Contains(PhysicsAnchorLocation))
                    _currTile = null;
                return _currTile;
            }
            set => _currTile = value;
        }

        private Color _ambient;
        private float _ambientIntensity;

        public Vector2 Velocity;
        public Vector2 Acceleration;
        private TerrainTile _currTile;

        public bool PhysicsActivated { get; set; }
        public Point PhysicsAnchorLocation
        {
            get
            {
                return Location.ToPoint() + new Point(Hitbox.Center.X, Hitbox.Height);
            }
        }

        public override Color Ambience {
            get
            {
                if (_ambient == Color.Transparent)
                    return Parent?.AmbientColor ?? Color.Transparent;
                return _ambient;
            }
            set
            {
                _ambient = value;
            }
        }
        public override float AmbientIntensity {
            get
            {
                if (_ambientIntensity == -1f)
                    return Parent?.AmbientIntensity ?? 0f;
                return _ambientIntensity;
            }
            set
            {
                _ambientIntensity = value;
            }
        }

        public EngineGameObject(string ID) : base(ID)
        {

        }

        public abstract EngineGameObject Create();

        public T GetContent<T>(string assetName)
        {
            var content = ProviderManager.GetRoot().Get<Content.LazerContentManager>();
            return content.GetContent<T>(assetName);
        }              
        
        public void ActivatePhysics()
        {
            var provider = ProviderManager.Root.Get<CollisionProvider>();
            provider.ActivateCollision(this);
        }

        /// <summary>
        /// Called when this object lands on solid terrain
        /// </summary>
        /// <param name="Terrain">The terrain tile containing the data about the currently landed terrain</param>
        /// <param name="Time">When this event was triggered</param>
        public virtual void TerrainGrounded(TerrainTile Terrain, GameTime Time)
        {
            Velocity = new Vector2(Velocity.X, 0);
            CurrentTile = Terrain;
        }

        /// <summary>
        /// Called when this object landed on another object
        /// </summary>
        /// <param name="Object">The object landed on</param>
        /// <param name="Time">When this event was triggered</param>
        public virtual void StandingOnObject(EngineGameObject Object, GameTime Time)
        {
            if (Velocity.Y > 0)
            {
                Location.Y = Object.Location.Y - Hitbox.Height;
                Velocity.Y = 0;                
                Acceleration.Y = -Parent.Gravity;
            }
            Velocity.X = Object.Velocity.X;
        }

        /// <summary>
        /// Called when this object touched the side of another object
        /// </summary>
        /// <param name="Object">The object touched</param>
        /// <param name="Time">When this event was triggered</param>
        /// <param name="WallDirection">The facing direction of the wall that this Object is touching</param>
        public virtual void ObjectSideTouched(EngineGameObject Object, GameTime Time, Direction WallDirection)
        {
            switch (WallDirection)
            {
                case Direction.EAST:
                    if (Velocity.X < 0)                    
                        Location.X = Object.Hitbox.Right;                                            
                    break;
                case Direction.WEST:
                    if (Velocity.X > 0)                    
                        Location.X = Object.Hitbox.Left - Hitbox.Width;
                    break;
            }
            Velocity.X = 0;
            Acceleration.X = 0;
        }

        /// <summary>
        /// Called when this object touched the bottom of another object
        /// </summary>         
        /// <param name="Object">The object touched</param>
        /// <param name="Time">When this event was triggered</param>
        public virtual void ObjectCeilingTouched(EngineGameObject Object, GameTime Time)
        {
            //if (Velocity.Y < 0)
            {
                Location.Y = Object.Hitbox.Bottom;
                Velocity.Y = 0;
                Acceleration.Y = -Parent.Gravity;
            }
        }

        public override void Update(GameTime gt)
        {
            IsOnScreen = Parent.Bounds.Bounds.Intersects(TextureDestination);
            var delta = gt.ElapsedGameTime.TotalSeconds;
            Acceleration = new Vector2(Acceleration.X, -Parent.Gravity);
            Velocity += Acceleration * new Vector2((float)delta);
            Location += Velocity;
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                if (ProviderManager.Root.Get<GameObjectProvider>().Ensure(new Person("debug_object")
                {
                    Texture = GameResources.BaseTexture,
                    Hitbox = new Rectangle(10, 10, 100, 100),
                    Color = Color.Gray
                }, out var added))
                    added.ActivatePhysics();
        }        

        public override void Draw(SpriteBatch batch)
        {
            //if(IsOnScreen)
                base.Draw(batch);
        } 
    }
}
