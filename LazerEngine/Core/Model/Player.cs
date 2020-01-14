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
    public class Player : Person
    {
        public Player() : base("player1")
        {
            AccelerationSpeed = new Vector2(2, 5);
        }

        public Player(string ID = "player1") : base(ID)
        {

        }

        public override EngineGameObject Create()
        {
            return new Player();
        }        

        public override void Load()
        {            
            base.Load();
        }

        public override void Update(GameTime gt)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Space))
                Location = new Vector2();
            if (keyboard.IsKeyDown(Keys.W))
                Velocity.Y = -10;
            if (keyboard.IsKeyDown(Keys.A))
                Acceleration.X = -AccelerationSpeed.X;
            else if (keyboard.IsKeyDown(Keys.D))
                Acceleration.X = AccelerationSpeed.X;
            else if (Math.Abs(Velocity.X) > 0)
            {
                Acceleration.X = Velocity.X > 0 ? -AccelerationSpeed.X : AccelerationSpeed.X;
                if (Math.Abs(Velocity.X) < Acceleration.X)
                {
                    Velocity.X = 0;
                    Acceleration.X = 0;
                }
            }
            base.Update(gt);
        }
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
    }
}
