using LazerEngine.Common.Provider;
using LazerEngine.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Provider
{
    public class ScreenProvider : IProvider
    {
        public ProviderManager Parent { get; set; }
        private Dictionary<string, Screen> _screens = new Dictionary<string, Screen>();
        public ScreenProvider()
        {

        }

        public Screen Add(Screen screen)
        {
            _screens.Add(screen.Name, screen);
            return _screens[screen.Name];
        }

        public Screen Get(string key)
        {
            if (_screens.TryGetValue(key, out var scr))
                return scr;
            return null;
        }

        public Screen[] GetAll()
        {
            return _screens.Values.ToArray();
        }

        public Screen Create(string key)
        {
            Add(new Screen(key));
            Console.WriteLine("Screen: " + key + " created");
            return _screens[key];
        }        

        public bool Remove(string key)
        {
            if (_screens.ContainsKey(key))
            {
                _screens.Remove(key);
                return true;
            }
            return false;
        }
    }
}
