using LazerEngine.Common.Model;
using LazerEngine.Common.Provider;
using LazerEngine.Content;
using LazerEngine.Core.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Provider
{
    public class GameObjectProvider : IProvider
    {
        public Screen Parent;
        ProviderManager IProvider.Parent { get => null; set { } }
        private Dictionary<string, EngineGameObject> _objects = new Dictionary<string, EngineGameObject>();
        public GameObjectProvider(Screen GameScreen)
        {
            Parent = GameScreen;
            Console.WriteLine("GameScreenProvider created for Screen: " + GameScreen.Name);
        }
        public EngineGameObject Get(string ID)
        {
            if (_objects.TryGetValue(ID, out var obj))
                return obj;
            return null;
        }
        public EngineGameObject[] GetAll()
        {
            return _objects.Values.ToArray();
        }
        public EngineGameObject Add(EngineGameObject obj)
        {
            _objects.Add(obj.ID, obj);
            obj.Parent = Parent;
            return obj;
        }
        public bool Ensure(EngineGameObject obj, out EngineGameObject added)
        {
            added = obj;
            if (_objects.ContainsKey(obj.ID) != true)
            {
                Add(obj);
                return true;
            }
            return false;
        }
        public T Create<T>(string id = null) where T : EngineGameObject, new()
        {
            if (id == null)
                id = EngineGameObject.GetID('O', _objects.Values);
            var obj = new T().Create();
            obj.Parent = Parent;
            Add(obj);
            Console.WriteLine($"GameObject: {id} created on Screen: {Parent.Name}");
            return (T)obj;
        }
        public T Create<T>(string textureName, string id = null, Rectangle hitbox = default, Color color = default) where T : EngineGameObject, new()
        {
            var obj = Create<T>(id);
            obj.Texture = ProviderManager.Root.Get<LazerContentManager>().GetContent<Texture2D>(textureName);
            if (hitbox == default)
                hitbox = obj.Texture.Bounds;
            obj.Hitbox = hitbox;
            if (color == default)
                color = Color.White;
            obj.Color = color;
            return obj;
        }
        public T Create<T>(Texture2D textureName, string id = null, Rectangle hitbox = default, Color color = default) where T : EngineGameObject, new()
        {
             var obj = Create<T>(id);
            obj.Texture = textureName;
            if (hitbox == default)
                hitbox = obj.Texture.Bounds;
            obj.Hitbox = hitbox;
            if (color == default)
                color = Color.White;
            obj.Color = color;
            return obj;
        }
        public bool Remove(string ID)
        {
            return _objects.Remove(ID);
        }
        public bool Remove(EngineGameObject obj)
        {
            return Remove(obj.ID);
        }
    }
}
