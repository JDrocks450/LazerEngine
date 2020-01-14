using LazerEngine.Common.Primitives;
using LazerEngine.Common.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model.Physics
{
    public class CollisionGroup<T> : IEnumerable<T>
    {
        public T North, East, South, West;

        public IEnumerator<T> GetEnumerator()
        {
            return new List<T>() { North, East, South, West }.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new T[] { North, East, South, West }.GetEnumerator();
        }
    }

    public class CollisionSensor
    {
        public Direction SenseDirection; 
        public Point Location { get; private set; }
        public int Size { get; private set; }

        /// <summary>
        /// The activation state since Refresh was called. 
        /// Updated when <see cref="GetActivateStateByObjects(Rectangle, IEnumerable{KeyValuePair{EngineGameObject, CollisionGroup{CollisionField}}}, out EngineGameObject[])"/> is called.
        /// </summary>
        public bool Activated { get; set; }
        public CollisionSensor(Direction SenseDir)
        {
            SenseDirection = SenseDir;            
        }                

        public void Refresh(Rectangle Hitboxrect)
        {
            switch (SenseDirection)
            {
                case Direction.NORTH:
                    Location = Hitboxrect.Location;
                    Size = Hitboxrect.Width;
                    break;
                case Direction.SOUTH:
                    Location = new Point(Hitboxrect.X, Hitboxrect.Bottom);
                    Size = Hitboxrect.Width;
                    break;
                case Direction.WEST:
                    Location = Hitboxrect.Location;
                    Size = Hitboxrect.Height;
                    break;
                case Direction.EAST:
                    Location = Hitboxrect.Location + new Point(Hitboxrect.Width, 0);
                    Size = Hitboxrect.Height;
                    break;
            }
            Activated = false;
        }

        /// <summary>
        /// Senses if any objects are touching this sensor
        /// </summary>
        /// <param name="HitboxArea"></param>
        /// <param name="checkThrough"></param>
        public bool GetActivateStateByObjects(Rectangle HitboxArea, IEnumerable<KeyValuePair<EngineGameObject, CollisionGroup<CollisionField>>> checkThrough, out EngineGameObject[] Touched)
        {
            Touched = null;
            var touched = new List<EngineGameObject>();
            foreach (var pair in checkThrough)
            {
                var obj = pair.Key;
                var group = pair.Value;
                CollisionField field = null;
                switch (SenseDirection)
                {
                    case Direction.NORTH:
                        field = group.South;
                        break;
                    case Direction.SOUTH:
                        field = group.North;
                        break;
                    case Direction.WEST:
                        field = group.East;
                        break;
                    case Direction.EAST:
                        field = group.West;
                        break;
                }
                field.Refresh(pair.Key.Hitbox);
                if (SenseField(field))
                {
                    Activated = true;
                    touched.Add(obj);
                }
            }
            if (touched.Any())
                Touched = touched.ToArray();
            return touched.Any();
        }

        public bool SenseField(CollisionField field)
        {
            bool[] sensed = new bool[2];
            int count = 0;
            foreach (var rect in field.GetFieldAreas())
            {
                sensed[count] = rect.Intersects(GetSensorArea());
                count++;
            }
            return sensed.Any(x => x);
        }

        public Rectangle GetSensorArea()
        {
            switch (SenseDirection)
            {
                case Direction.NORTH:
                case Direction.SOUTH:
                    return new Rectangle(Location, new Point(Size, 5));
                case Direction.EAST:
                case Direction.WEST:
                    return new Rectangle(Location, new Point(5, Size));                    
            }
            return default; // impossible but ok
        }

        public void DebugDraw(SpriteBatch batch)
        {
            switch (SenseDirection)
            {
                case Direction.NORTH:
                case Direction.SOUTH:
                    batch.Draw(GameResources.BaseTexture, new Rectangle(Location, new Point(Size, 5)), SenseDirection == Direction.NORTH ? Color.Blue : Color.Red);
                    break;
                case Direction.EAST:
                case Direction.WEST:
                    batch.Draw(GameResources.BaseTexture, new Rectangle(Location, new Point(5, Size)), Color.Green);
                    break;
            }            
        }
    }
}
