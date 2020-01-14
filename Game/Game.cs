using Game.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : LazerEngine.LazerGameCore
    {
        public Game() : base()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {            
            var screen = ScreenProvider.Add(new Gameplay());
            screen.AddParalaxTexture(ContentManager.GetContent<Texture2D>("Level_Cavern/far"));
            screen.AddParalaxTexture(ContentManager.GetContent<Texture2D>("Level_Cavern/sand"));
            screen.AddParalaxTexture(ContentManager.GetContent<Texture2D>("Level_Cavern/foreground-merged"));            
            screen.Load();            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
    }
}
