using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using PolyOne;
using PolyOne.Collision;
using PolyOne.Components;
using PolyOne.Engine;
using PolyOne.Scenes;

namespace PLWing
{
    public class AirEnemy : Entity
    {
        private Texture2D plane;
        private StateMachine state;

        public AirEnemy(Vector2 position)
            : base(position)
        {
            this.Tag((int)GameTags.AirEnemy);
            this.Collider = new Hitbox((float)24.0f, (float)24.0f, -12.0f, -12.0f);
            this.Visible = true;

            plane = Engine.Instance.Content.Load<Texture2D>("EnemyAssets/Enemy");

            //state = new StateMachine(0);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();

            Engine.SpriteBatch.Draw(plane, Position, Color.White);
        }
    }
}
