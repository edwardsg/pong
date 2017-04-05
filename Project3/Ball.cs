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
            position += update;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void UpdateBall(float timePassed, Box player1, Box player2, Matrix boundingBoxWorld)
        {
            position += velocity * timePassed;
            
            // If ball is at the X bounds of the box at the side with player 1
            if (position.Z > boundingBoxWorld.M33 - player1.getShapeDimensions().Z)
                checkPlayer(player1.getPosition());

            // If ball is at the X bounds of the box at the side with player 2
            if (position.Z < -boundingBoxWorld.M33 + player2.getShapeDimensions().Z)
                checkPlayer(player2.getPosition());

            if (position.Y > boundingBoxWorld.M22 - 1 || position.Y < -boundingBoxWorld.M22 + 1)
                velocity.Y *= -1;

            if (position.X > boundingBoxWorld.M11 - 1 || position.X < -boundingBoxWorld.M11 + 1)
                velocity.X *= -1;
        }

        private void checkPlayer(Vector3 playerPosition)
        {
            // If the position of the ball is within the bounds of the position of the paddle
            if (position.X <= playerPosition.X + 2f && position.X >= playerPosition.X - 2f &&
                position.Y <= playerPosition.Y + 2f && position.Y >= playerPosition.Y - 2f)
            {
                float xDifference = position.X - playerPosition.X;
                float yDifference = position.Y - playerPosition.Y;
                float zDifference = position.Z - playerPosition.Z;
                velocity.Normalize();

                velocity += new Vector3(xDifference, yDifference, zDifference);
                velocity.Normalize();
                velocity *= 1;
                velocity.Z *= -1;
            }

            // Else the ball went out of the bounds and should be reset
            else
            {
                position = Vector3.Zero;
                velocity = new Vector3(0, 0, 1f);
            }
        }

        public void callDraw(GraphicsDeviceManager graphics, Matrix view, Matrix projection, Color color)
        {
            world = Matrix.CreateScale(1) * Matrix.CreateTranslation(position);
            DrawShape(graphics, world, view, projection, basicEffect, color, sphere);
        }
    }
}
