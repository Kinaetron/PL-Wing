﻿using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using PolyOne.ScreenManager;
using PolyOne.Input;
using PolyOne.Utility;
using PolyOne.Engine;

namespace PLWing.Screens
{
    class GameplayScreen : GameScreen
    {
        private float pauseAlpha;
        private Level level = new Level();

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ScreenManager.Game.ResetElapsedTime();
            level.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (coveredByOtherScreen) {
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            }
            else {
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            }
               

            if (IsActive == true)
            {
                level.Update();
            }
        }

        public override void HandleInput(InputMenuState input)
        {
            if (input.IsPauseGame(ControllingPlayer)) {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            level.Draw();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
