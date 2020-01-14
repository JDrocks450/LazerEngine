using LazerEngine.Common.Primitives;
using LazerEngine.Common.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model.Physics
{
    public class CollisionField
    {
        public CollisionType Type
        {
            get;
            set;
        }
        public Direction WallPushDirection { get; set; }
        public Point Location { get; set; }
        public int Size { get; set; }

        private (bool enabled, Rectangle rectangle)[] sensorAreas = new (bool, Rectangle)[2];
        private Color debugDrawColor = Color.White;

        public CollisionField(CollisionType collisionType, Direction wallPushDirection = Direction.WEST)
        {
            Type = collisionType;
            if (wallPushDirection == Direction.NORTH || wallPushDirection == Direction.SOUTH)
                wallPushDirection = Direction.WEST;
            WallPushDirection = wallPushDirection;
            sensorAreas[0].enabled = true;
            sensorAreas[1].enabled = true;
        }

        /// <summary>
        /// Moves the Sensor to cover the edges of the Rectangle
        /// </summary>
        /// <param name=""></param>
        public void Refresh(Rectangle Hitrect)
        {
            switch (Type)
            {
                case CollisionType.FLOOR:
                    Location = Hitrect.Location;
                    Size = Hitrect.Width;
                    sensorAreas[0] = (sensorAreas[0].enabled,
                        new Rectangle(
                            Hitrect.Location - new Point(0, 25),
                            new Point(Hitrect.Width, 25)));
                    sensorAreas[1] = (sensorAreas[1].enabled,
                        new Rectangle(
                            Hitrect.Location,
                            new Point(Hitrect.Width, 15)));
                    debugDrawColor = Color.Blue;
                    break;
                case CollisionType.CEILING:
                    Size = Hitrect.Width;
                    Location = new Point(Hitrect.X, Hitrect.Bottom);
                    sensorAreas[0] = (sensorAreas[0].enabled,
                        new Rectangle(
                            new Point(Hitrect.X, Hitrect.Bottom) - new Point(0, Hitrect.Height - 15),
                            new Point(Hitrect.Width, Hitrect.Height - 15)));
                    sensorAreas[1] = (sensorAreas[1].enabled,
                        new Rectangle(
                            new Point(Hitrect.X, Hitrect.Bottom),
                            new Point(Hitrect.Width, 25)));
                    debugDrawColor = Color.Red;
                    break;
                case CollisionType.WALL:
                    debugDrawColor = Color.Green;
                    switch (WallPushDirection)
                    {
                        case Direction.WEST:
                            Location = new Point(Hitrect.X, Hitrect.Y);
                            Size = Hitrect.Height;
                            sensorAreas[0] = (sensorAreas[0].enabled,
                                new Rectangle(
                                    new Point(Hitrect.X, Hitrect.Y) - new Point(25, 0),
                                    new Point(25, Hitrect.Height)));
                            sensorAreas[1] = (sensorAreas[1].enabled,
                                new Rectangle(
                                    new Point(Hitrect.X, Hitrect.Y),
                                    new Point(25, Hitrect.Height)));
                            break;
                        case Direction.EAST:
                            Location = new Point(Hitrect.Right, Hitrect.Y);
                            Size = Hitrect.Height;
                            sensorAreas[0] = (sensorAreas[0].enabled,
                                new Rectangle(
                                    new Point(Hitrect.Right, Hitrect.Y) - new Point(25, 0),
                                    new Point(25, Hitrect.Height)));
                            sensorAreas[1] = (sensorAreas[1].enabled,
                                new Rectangle(
                                    new Point(Hitrect.Right, Hitrect.Y),
                                    new Point(25, Hitrect.Height)));
                            break;
                    }
                    break;
            }
        }

        public Rectangle[] GetFieldAreas()
        {
            return sensorAreas.Where(x => x.enabled).Select(x => x.rectangle).ToArray();
        }

        public void DebugDraw(SpriteBatch batch)
        {            
            switch (Type)
            {
                case CollisionType.FLOOR:
                case CollisionType.CEILING:                   
                    batch.Draw(GameResources.BaseTexture, sensorAreas[0].rectangle, debugDrawColor * .5f);
                    batch.Draw(GameResources.BaseTexture, sensorAreas[1].rectangle, debugDrawColor * .5f);
                    //batch.Draw(GameResources.BaseTexture, new Rectangle(Location, new Point(Size, 5)), debugDrawColor);
                    break;
                case CollisionType.WALL:                    
                    batch.Draw(GameResources.BaseTexture, sensorAreas[0].rectangle, debugDrawColor * .5f);
                    batch.Draw(GameResources.BaseTexture, sensorAreas[1].rectangle, debugDrawColor * .5f);
                    //batch.Draw(GameResources.BaseTexture, new Rectangle(Location, new Point(5, Size)), debugDrawColor);
                    break;
            }   
        }
    }
}
