using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Common.Model
{
    public interface IGameComponent : IDisposable
    {        
        /// <summary>
        /// Load will be ignored if this is true
        /// </summary>
        bool IsLoaded
        {
            get; set;
        }
        void Load();
        void Update(GameTime gt);
        void Draw(SpriteBatch batch);        
    }
}
