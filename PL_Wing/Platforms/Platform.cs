using Microsoft.Xna.Framework;

using PolyOne;

namespace PLWing.Platforms
{
    public abstract class Platform : Entity
    {
        public Platform(Vector2 position)
            : base(position)
        {
        }
    }
}
