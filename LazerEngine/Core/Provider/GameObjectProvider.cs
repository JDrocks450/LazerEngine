using LazerEngine.Common.Model;
using LazerEngine.Common.Provider;
using LazerEngine.Core.Model;
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
        public void Add(EngineGameObject obj)
        {
            _objects.Add(obj.ID, obj);
        }
        public T Create<T>(string id = null) where T : EngineGameObject, new()
        {
            if (id == null)
                id = EngineGameObject.GetID('O', _objects.Values);
            var obj = new T().Create();
            Add(obj);
            Console.WriteLine($"GameObject: {id} created on Screen: {Parent.Name}");
            return (T)obj;
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
