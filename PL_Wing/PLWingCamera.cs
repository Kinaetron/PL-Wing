using System;
using Microsoft.Xna.Framework;
using PolyOne.Utility;

namespace PLWing
{
    public class PLWingCamera : Camera
    {
        public void LockToTarget(Vector2 position, int screenWidth, int screenHeight)
        {
            Position.X = position.X - screenWidth / 2;
            Position.Y = position.Y - screenHeight / 2;
        }
    }
}
