using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primitives;
using Microsoft.Xna.Framework.Audio;

namespace Project3
{
    class Ball : Shape
    {
        SpherePrimitive sphere;

        float radius = 1;

        BasicEffect basicEffect;
        Matrix world;

        Vector3 position;
        Vector3 velocity;

        SoundEffect sound;

        public Ball(GraphicsDeviceManager graphics, Vector3 position, Vector3 velocity)
        {
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            sphere = new SpherePrimitive(graphics.GraphicsDevice);
            this.position = position;
            this.velocity = velocity;
        }

        public void setSound(SoundEffect sound)
        {
            this.sound = sound;
        }

        public void setPosition(Vector3 update)
        {
            position += update;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void setVelocity(Vector3 update)
        {
            velocity += update;
        }

        public Vector3 getVelocity()
        {
            return velocity;
        }

        // TODO: Still can have occasional glancing blows that cause errors in paddle and ball update
        public bool UpdateBall(float timePassed, Box player1, Box player2, Box helper, Vector3 boundingBoxWorld)
        {
            position += velocity * timePassed;

            // If ball is at the Z bounds of the box at the side with player 1
            if (position.Z > boundingBoxWorld.Z - radius)
            {
                sound.Play(;
                return checkPlayer(player1.getPosition(), helper);
            }

            // If ball is at the Z bounds of the box at the side with player 2
            if (position.Z < -boundingBoxWorld.Z + radius)
            {
                sound.Play();
                return checkPlayer(player2.getPosition(), helper);
            }

            if (position.Y > boundingBoxWorld.Y - radius || position.Y < -boundingBoxWorld.Y + radius)
            {
                velocity.Y *= -1;
                sound.Play();
            }

            if (position.X > boundingBoxWorld.X - radius || position.X < -boundingBoxWorld.X + radius)
            {
                velocity.X *= -1;
                sound.Play();
            }

            return false;
        }

        private bool checkPlayer(Vector3 playerPosition, Box helper)
        {
            // If the position of the ball is within the bounds of the position of the paddle
            if (position.X <= playerPosition.X + 1f && position.X >= playerPosition.X - 1f &&
                position.Y <= playerPosition.Y + 1f && position.Y >= playerPosition.Y - 1f)
            {
                float xDifference = position.X - playerPosition.X;
                float yDifference = position.Y - playerPosition.Y;
                velocity.Normalize();

                velocity += new Vector3(xDifference, yDifference, 0);
                velocity.Normalize();
                velocity *= 1;
                velocity.Z *= -1;

                return true;
            }

            // Else the ball went out of the bounds and should be reset
            else
            {
                position = Vector3.Zero;
                velocity = new Vector3(0, 0, 1f);
                helper.setPosition(new Vector3(0, 0, 20));
                return false;
            }
        }

        public void callDraw(GraphicsDeviceManager graphics, Matrix view, Matrix projection, Color color)
        {
            world = Matrix.CreateScale(1) * Matrix.CreateTranslation(position);
            DrawShape(graphics, world, view, projection, basicEffect, color, sphere);
        }
    }
}
