using LazerEngine.Common.Model;
using LazerEngine.Common.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine.Core.Model
{
    public abstract class EngineGameObject : LazerGameComponent
    {
        public bool IsOnScreen;
        public Screen Parent;

        public EngineGameObject(string ID) : base(ID)
        {
        }

        public abstract EngineGameObject Create();

        public T GetContent<T>(string assetName)
        {
            var content = ProviderManager.GetRoot().Get<Content.LazerContentManager>();
            return content.GetContent<T>(assetName);
        }               

        public override void Update(GameTime gt)
        {
            IsOnScreen = Parent.Bounds.Bounds.Intersects(TextureDestination);
        }

        public override void Draw(SpriteBatch batch)
        {
            if(IsOnScreen)
                base.Draw(batch);
        } 
    }
}
