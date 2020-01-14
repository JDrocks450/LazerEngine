using LazerEngine.Common.Model;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model
{
    public class Person : EngineGameObject
    {
        public Vector2 MaxSpeed = new Vector2(3);
        public Vector2 AccelerationSpeed;
        public Person(string ID) : base(ID)
        {

        }

        public override EngineGameObject Create()
        {
            return new Person(null);
        }

        public override void Load()
        {
            ;
        }

        public override void Update(GameTime gt)
        {
            if (Velocity.X + Acceleration.X > MaxSpeed.X && Acceleration.X != 0)
            {                
                Velocity.X = (Acceleration.X < 0) ? -MaxSpeed.X : MaxSpeed.X;
                Acceleration.X = 0;
            }
            base.Update(gt);
        }
    }
}
