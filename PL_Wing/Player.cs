using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Input;
using PolyOne.Components;

namespace PLWing
{
    public class Player : Entity
    {
        public PLWingCamera Camera { get; private set; }
        private Level level;

        private const float deadZone = 0.5f;

        private Texture2D playerTexture;

        private const float gravityAccel = 0.1f;

        private const float accerlation = 0.6f;
        private const float maxSpeed = 6.0f;
        private const float fallSpeed = 4.0f;
        private const float frictionA = 0.09f;
        private const float frictionO = 0.03f;
        private Vector2 remainder;
        private Vector2 velocity;


        private const float angleSpeedOff = 0.06f;
        private const float angleSpeedOn = 0.03f;
        private float returnAngle;
        private float previousAngle;
        private Vector2 directionAngle;

        private const float shootCoolDown = 150.0f;

        private float rotation;

        private CounterSet<string> counters = new CounterSet<string>();

        private List<Buttons> shootButtons = new List<Buttons>(new Buttons[] 
                                                    { Buttons.A, Buttons.B, Buttons.X, Buttons.Y });

        private const int healthCount = 3;
        private Texture2D healthPointTexture;
        private Vector2 healthPosition = new Vector2(10, 10);

        public Player(Vector2 position)
            : base(position)
        {
            this.Tag((int)GameTags.Player);
            this.Collider = new Hitbox((float)24.0f, (float)24.0f, -12.0f, -12.0f);
            this.Visible = true;

            playerTexture = Engine.Instance.Content.Load<Texture2D>("Plane");
            healthPointTexture = Engine.Instance.Content.Load<Texture2D>("HealthPoint");

            Camera = new PLWingCamera();

            this.Add(counters);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (base.Scene is Level) {
                this.level = (base.Scene as Level);
            }
        }

        private float StickDirection()
        {
            float angle = (float)Math.Atan2(-PolyInput.GamePads[0].GetLeftStick(deadZone).Y,
                                             PolyInput.GamePads[0].GetLeftStick(deadZone).X);

           if(PolyInput.GamePads[0].GetLeftStick(deadZone) != Vector2.Zero) {
                previousAngle = angle;
           }
           else {
                angle = previousAngle;
           }

           if (PolyInput.GamePads[0].LeftTriggerCheck(0.3f) == true ||
               PolyInput.GamePads[0].RightTriggerCheck(0.3f) == true)
            {
                returnAngle = CurveAngle(returnAngle, angle, angleSpeedOn);
            }
           else {
                returnAngle = CurveAngle(returnAngle, angle, angleSpeedOff);
            }

            directionAngle = new Vector2((float)Math.Cos(returnAngle), (float)Math.Sin(returnAngle));

            return returnAngle;
        }

        private float CurveAngle(float from, float to, float step)
        {
            if (step == 0) {
                return from;
            }
               
            if (from == to || step == 1) {
                return to;
            }

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = Slerp(fromVector, toVector, step);

            return (float)Math.Atan2(currentVector.Y, currentVector.X);
        }

        private Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) {
                return from;
            }

            if (from == to || step == 1) {
                return to;
            }

            double theta = Math.Acos(Vector2.Dot(from, to));

            if (theta == 0) {
                return to;
            }

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;
        }


        public override void Update()
        {
            foreach (Buttons button in shootButtons)
            {
                if (PolyInput.GamePads[0].Check(button) == true && 
                    counters["shootTimer"] <= 0)
                {
                    counters["shootTimer"] = shootCoolDown;
                    Shoot();
                }
            }

            Camera.LockToTarget(this.Position, Engine.VirtualWidth, Engine.VirtualHeight);
            Camera.ClampToArea((int)level.Tile.MapWidthInPixels - Engine.VirtualWidth, (int)level.Tile.MapHeightInPixels - Engine.VirtualHeight);

            if (PolyInput.GamePads[0].LeftTriggerCheck(0.3f) == true || 
                PolyInput.GamePads[0].RightTriggerCheck(0.3f) == true)
            {
                Vector2 normalDirection = directionAngle;
                normalDirection.Normalize();

                velocity += normalDirection * accerlation;

                velocity.X = MathHelper.Lerp(velocity.X, 0, frictionA);
                velocity.Y = MathHelper.Lerp(velocity.Y, 0, frictionA);
           
                velocity.Y = MathHelper.Clamp(velocity.Y, -maxSpeed, maxSpeed);
            }
            else
            {
                velocity.Y += gravityAccel;
                velocity.X = MathHelper.Lerp(velocity.X, 0, frictionO);

                velocity.Y = MathHelper.Clamp(velocity.Y, -fallSpeed, fallSpeed);
            }

            velocity.X = MathHelper.Clamp(velocity.X, -maxSpeed, maxSpeed);

            MovementHorizontal(velocity.X);
            MovementVerical(velocity.Y);

            rotation = StickDirection();

            base.Update();
        }

        public void Shoot()
        {
            Bullet bullet;

            bullet = new Bullet(Position, directionAngle);
            this.Scene.Add(bullet);
            bullet.Added(this.Scene);
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
                        velocity.X = 0.0f;
                        remainder.X = 0;
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
                        velocity.Y = 0.0f;
                        remainder.Y = 0;
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
                        remainder.Y = 0;
                        break;
                    }
                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Engine.SpriteBatch.Draw(playerTexture, Position, null, Color.White, rotation, new Vector2(12, 12) , 1.0f, SpriteEffects.None, 0.0f);

            for (int i = 0; i < healthCount; i++) {
                Engine.SpriteBatch.Draw(healthPointTexture, (healthPosition + new Vector2(Camera.Position.X + i * 10, Camera.Position.Y)), Color.White);
            }
        }
    }
}
