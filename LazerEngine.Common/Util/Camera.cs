using LazerEngine.Common.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Common.Util
{
    public class Camera
    {
        protected float _zoom; // Camera Zoom
        protected float _rotation; //Camera Rotation
        public Matrix _transform; // Matrix Transform

        public bool CanMoveBackwards = true;

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public Vector2 Pos;

        public Rectangle Screen;

        private LazerGameComponent _focus;
        public LazerGameComponent Focus
        {
            get => _focus;
            set
            {
                if (value != _focus)
                {
                    _focus = value;
                    MoveToFocus();
                }
            }
        }

        /// <summary>
        /// Freezes the Camera's Position
        /// </summary>
        public bool Frozen { get; set; } = false;
        /// <summary>
        /// The speed the Camera moves when focusing on an object
        /// </summary>
        public int DriftSpeed { get; set; } = 5;
        private bool _movingToFocus = false;        

        public void MoveToFocus(int speed = default)
        {
            if (speed != default)
                DriftSpeed = speed;         
            _movingToFocus = true;
        }

        void doMoveToFocus(Vector2 newCenter)
        {
            if (Pos.X != newCenter.X)
            {
                if (Pos.X + DriftSpeed < newCenter.X)
                    Pos.X += DriftSpeed;
                else if (Pos.X - DriftSpeed > newCenter.X)
                    Pos.X -= DriftSpeed;
                else
                    goto cont;
                if (Pos.Y != newCenter.Y)
                    if (Pos.Y+DriftSpeed < newCenter.Y)
                        Pos.Y += DriftSpeed;
                    else if(Pos.Y-DriftSpeed > newCenter.Y)
                        Pos.Y -= DriftSpeed;
                return;
            }
            cont:
            Pos = newCenter;
            _movingToFocus = false;
        }

        /// <summary>
        /// Creates a matrix for the spritebatch that automatically focuses on the "Focus" if there is one set.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public Matrix Transform(GraphicsDevice graphicsDevice)
        {
            var player = Focus;
            var screen = graphicsDevice.Viewport;
            if (player != null)
            {
                var center = player.Location + player.TextureDestination.Center.ToVector2();
                center += new Vector2(screen.Width, screen.Height) / 2;
                if (_movingToFocus)
                    doMoveToFocus(center);
                else
                {
                    if (!CanMoveBackwards)
                    {
                        if (center.X > Pos.X)
                            Pos.X = center.X;
                    }
                    else
                        Pos.X = center.X;
                    Pos.Y = center.Y;
                }                
            }    
            Screen = new Rectangle((int)Pos.X - screen.Width, (int)Pos.Y - screen.Height, screen.Width, screen.Height);
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                                         Matrix.CreateRotationZ(0) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(0, 0, 0));
            return _transform;
        }

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            Pos = Vector2.Zero;
        }
    }
}
