using LazerEngine.Common.Provider;
using LazerEngine.Core.Model;
using LazerEngine.Core.Model.Physics;
using LazerEngine.Core.Model.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Provider
{
    public class CollisionProvider : IProvider
    {
        private Dictionary<EngineGameObject, CollisionGroup<CollisionSensor>> _sensors = new Dictionary<EngineGameObject, CollisionGroup<CollisionSensor>>();
        private Dictionary<EngineGameObject, CollisionGroup<CollisionField>> _solids = new Dictionary<EngineGameObject, CollisionGroup<CollisionField>>();

        public CollisionProvider()
        {

        }

        public ProviderManager Parent { get; set; }

        /// <summary>
        /// Invites this Object into the Collision System
        /// </summary>
        /// <param name="Object">The Object to verify collision with</param>
        /// <param name="IsSolid">Can other objects pass through this one?</param>
        public void ActivateCollision(EngineGameObject Object, bool IsSolid = true)
        {
            _sensors.Add(Object, new CollisionGroup<CollisionSensor>()
            {
                North = new CollisionSensor(Common.Primitives.Direction.NORTH),
                South = new CollisionSensor(Common.Primitives.Direction.SOUTH),
                East = new CollisionSensor(Common.Primitives.Direction.EAST),
                West = new CollisionSensor(Common.Primitives.Direction.WEST)
            });
            if (IsSolid)
                _solids.Add(Object, new CollisionGroup<CollisionField>()
                {
                    North = new CollisionField(Common.Primitives.CollisionType.FLOOR),
                    South = new CollisionField(Common.Primitives.CollisionType.CEILING),
                    East = new CollisionField(Common.Primitives.CollisionType.WALL, Common.Primitives.Direction.EAST),
                    West = new CollisionField(Common.Primitives.CollisionType.WALL, Common.Primitives.Direction.WEST)
                });
        }

        /// <summary>
        /// Removes the collision for this Object from the Collision System
        /// </summary>
        /// <param name="Object"></param>
        public void DeactivateCollision(EngineGameObject Object)
        {
            _sensors.Remove(Object);
            _solids.Remove(Object);
        }

        public CollisionGroup<CollisionSensor> GetSensors(EngineGameObject Object)
        {
            return _sensors[Object];
        }
        public CollisionGroup<CollisionField> GetSolids(EngineGameObject Object)
        {
            return _solids[Object];
        }

        /// <summary>
        /// Checks collision, corrects positioning, and calls Physics related events for each object in the Collision System (called once per frame)
        /// </summary>
        public void UpdateCollision(Screen GameScreen, GameTime gameTime)
        {
            var terrain = TerrainProvider.GetLayer(GameScreen.Name, 1);
            foreach(var sensorObject in _sensors)
            {
                foreach(var sensor in sensorObject.Value)
                {
                    sensor.Refresh(sensorObject.Key.Hitbox);
                    var activated = sensor.GetActivateStateByObjects(sensorObject.Key.Hitbox, _solids.Where(x => x.Key != sensorObject.Key), out var objArr);
                    if (objArr == null)
                        continue;
                    foreach (var obj in objArr) {
                        switch (sensor.SenseDirection)
                        {
                            case Common.Primitives.Direction.NORTH:
                                sensorObject.Key.ObjectCeilingTouched(obj, gameTime);
                                break;
                            case Common.Primitives.Direction.WEST:
                                sensorObject.Key.ObjectSideTouched(obj, gameTime, Common.Primitives.Direction.EAST);
                                break;
                            case Common.Primitives.Direction.EAST:
                                sensorObject.Key.ObjectSideTouched(obj, gameTime, Common.Primitives.Direction.WEST);
                                break;
                            case Common.Primitives.Direction.SOUTH:
                                sensorObject.Key.StandingOnObject(obj, gameTime);
                                break;
                        } 
                    }
                }
                var engineObj = sensorObject.Key;
                var terrainTiles = terrain.GetTilesByArea(engineObj.Hitbox);
                foreach (var terrainTile in terrainTiles)
                {
                    if (terrainTile != null && terrainTile.HasCollision)
                    {
                        var heights = terrain.GetHeightRangeScaled(terrainTile, engineObj.Hitbox.Left, engineObj.Hitbox.Right);
                        float snapHeight = terrainTile.GetWorldTileLocation().Y + heights.Select(x => x.high).Min();
                        if (engineObj.Hitbox.Bottom > snapHeight)
                        {
                            engineObj.Hitbox = new Rectangle(new Point((int)engineObj.Location.X,
                                (int)(snapHeight - engineObj.Hitbox.Height)), engineObj.Size.ToPoint());
                            engineObj.TerrainGrounded(terrainTile, gameTime);
                        }
                    }
                }
            }
        } 

        public void DebugDraw(SpriteBatch batch)
        {            
            foreach(var group in _solids.Values)
            {
                foreach (var sensor in group)
                    sensor.DebugDraw(batch);
            }
            foreach(var group in _sensors.Values)
            {
                foreach (var sensor in group)
                    sensor.DebugDraw(batch);
            }
        }
    }
}
