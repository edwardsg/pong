using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primitives;

namespace Project3
{
    class Ball : Shape
    {
        SpherePrimitive sphere;

        BasicEffect basicEffect;
        Matrix world;

        Vector3 position;
        Vector3 velocity;

        public Ball(GraphicsDeviceManager graphics, Vector3 position, Vector3 velocity)
        {
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            sphere = new SpherePrimitive(graphics.GraphicsDevice);
            this.position = position;
            this.velocity = velocity;
        }

        public void setPosition(Vector3 update)
        {
            position = update;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void UpdateBall(float timePassed, Box player1, Box player2)
        {
            position += velocity * timePassed;

            // Preliminarily hardcoded in the bounds for the ball movement; 19.5 is the box and the 1 accounts for the radius of the ball (TODO fix later)

            // If ball is at the X bounds of the box at the side with player 1
            if (position.X < -19.5f + 1)
                checkPlayer(player1.getPosition());

            // If ball is at the X bounds of the box at the side with player 2
            if (position.X > 19.5f - 1)
                checkPlayer(player2.getPosition());

            if (position.Y > 19.5f || position.Y < -19.5f)
                velocity.Y *= -1;

            if (position.Z > 19.5f || position.Z < -19.5f)
                velocity.Z *= -1;
        }

        private void checkPlayer(Vector3 playerPosition)
        {
            // If the position of the ball is within the bounds of the position of the paddle
            if (position.Z <= playerPosition.Z + 4f && position.Z >= playerPosition.Z - 4f &&
                position.Y <= playerPosition.Y + 4f && position.Y >= playerPosition.Y - 4f)
            {
                float xDifference = position.X - playerPosition.X;
                float yDifference = position.Y - playerPosition.Y;
                float zDifference = position.Z - playerPosition.Z;
                velocity.Normalize();

                velocity += new Vector3(xDifference, yDifference, zDifference);
                velocity.Normalize();
                velocity *= 1;
            }

            // Else the ball went out of the bounds and should be reset
            else
            {
                position = Vector3.Zero;
                velocity = new Vector3(-1f, 0, 0);
            }
        }

        public void callDraw(GraphicsDeviceManager graphics, Matrix view, Matrix projection, Color color)
        {
            world = Matrix.CreateScale(1) * Matrix.CreateTranslation(position);
            DrawShape(graphics, world, view, projection, basicEffect, color, sphere);
        }
    }
}
