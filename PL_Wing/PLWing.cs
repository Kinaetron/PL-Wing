using Microsoft.Xna.Framework;

using PolyOne.Engine;
using PolyOne.Utility;

using PLWing.Screens;

namespace PLWing
{
    public class PLWing : Engine
    {
        static readonly string[] preloadAssets =
        {
            "MenuAssets/gradient",
        };

        public PLWing()
            : base(640, 360, "Swordfish III", 2.0f, false)
        {
        }

        protected override void Initialize()
        {

            base.Initialize();

            TileInformation.TileDiemensions(32, 32);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (string asset in preloadAssets) {
                Engine.Instance.Content.Load<object>(asset);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            using (PLWing game = new PLWing())
            {
                game.Run();
            }
        }
    }
}
