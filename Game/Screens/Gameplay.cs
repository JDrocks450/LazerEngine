using LazerEngine.Common.Provider;
using LazerEngine.Core.Model;
using LazerEngine.Core.Provider;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Screens
{
    public class Gameplay : Screen
    {
        public Gameplay(string name = "gameplay") : base(name)
        {
        }        

        public override void Load()
        {
            base.Load();
            TerrainProvider.CreateLayer(Name, out var layer, 1, new Point(170));
            layer.SetTile(new Point(0,3), "Level_Cavern/Terrain/grass_brick");
            layer.SetTile(new Point(2,3), "Level_Cavern/Terrain/grass_brick");
            layer.SetTile(new Point(3,3), "Level_Cavern/Terrain/grass_brick");
            layer.SetTile(new Point(1,4), "Level_Cavern/Terrain/brick_all");
            layer.SetTile(new Point(2,4), "Level_Cavern/Terrain/brick_all");
            layer.SetTile(new Point(3,4), "Level_Cavern/Terrain/brick_all");
            var player = ProviderManager.Root.Get<GameObjectProvider>().Create<Player>("Characters/Ghost/idle", "player", default, Color.White);
            player.ActivatePhysics();
            AmbientColor = Color.DeepSkyBlue;
            AmbientIntensity = .25f;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void Draw(SpriteBatch batch)
        {            
            base.Draw(batch);
        }
    }
}
