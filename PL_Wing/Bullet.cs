using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Scenes;

namespace PLWing
{
    public class Bullet : Entity
    {
        private Level level;

        private float rotation;
        private const float speed = 15.0f;
        private Vector2 remainder;

        private Texture2D texture;

        private Vector2 direction;
        private Vector2 velocity;

        private Vector2 origin = new Vector2(6, 3);

        public Bullet(Vector2 position, Vector2 stickDirection)
               : base(position)
        {
            this.Tag((int)GameTags.Bullet);
            this.Collider = new Hitbox(12.0f, 6.0f, -6.0f, -3.0f);
            direction = stickDirection;
            this.Visible = true;

            texture = Engine.Instance.Content.Load<Texture2D>("Bullet");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (base.Scene is Level) {
                this.level = (base.Scene as Level);
            }
        }

        public override void Update()
        {
            base.Update();

            direction.Normalize();
            velocity = direction * speed;
            rotation = (float)Math.Atan2(direction.Y, direction.X);

            MovementHorizontal(velocity.X);
            MovementVerical(velocity.Y);
        }

        private void MovementHorizontal(float amount)
        {
            remainder.X += amount;
            int move = (int)Math.Round((double)remainder.X);

            if (move != 0)
            {
                remainder.X -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(sign, 0);

                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        this.RemoveSelf();
                        break;
                    }

                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        private void MovementVerical(float amount)
        {
            remainder.Y += amount;
            int move = (int)Math.Round((double)remainder.Y);

            if (move < 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, -1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        this.RemoveSelf();
                        break;
                    }
        
                    Position.Y += -1.0f;
                    move -= -1;
                }
            }
            else if (move > 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, 1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        this.RemoveSelf();
                        break;
                    }
                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0.0f);
            base.Draw();
        }
    }
}
