using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3
{
    class Box : Shape
    {
        BasicEffect basicEffect;
        Matrix world;
        float alphaChange; // For visibility of paddle when in front of the ball
        float radius = 1;

        Vector3 front = new Vector3(0, 0, 20 - 1);
        Vector3 back = new Vector3(0, 0, -20 + 1);
        Vector3 right = new Vector3(10 - 1, 0, 0);
        Vector3 left = new Vector3(-10 + 1, 0, 0);
        Vector3 top = new Vector3(0, 10 - 1, 0);
        Vector3 bottom = new Vector3(0, -10 + 1, 0);

        Vector3 position;
        Vector3 shapeDimensions;

        public Box(GraphicsDeviceManager graphics, Vector3 position, Vector3 shapeDimensions)
        {
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            this.position = position;
            this.shapeDimensions = shapeDimensions;
        }

        public void setPosition(Vector3 update)
        {
            position = update;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void setShapeDimensions(Vector3 dimensions)
        {
            shapeDimensions = dimensions;
        }

        public Vector3 getShapeDimensions()
        {
            return shapeDimensions;
        }

        // To do recursive computations for the position of the AI
        public Vector3 detectCollision(Vector3 ballPosition, Vector3 ballVelocity)
        {
            Vector3 collision = Vector3.Zero;

            // Case where need to check if ball hits plane
            float temp = Vector3.Dot(-Vector3.UnitZ, ballVelocity);
            if (Vector3.Dot(-Vector3.UnitZ, ballVelocity) < 0)
            {
                float time = (front.Z - ballPosition.Z) / ballVelocity.Z;
                collision = ballPosition + ballVelocity * time;

                if (withinBounds(collision))
                    return collision;
            }

            if (Vector3.Dot(Vector3.UnitZ, ballVelocity) < 0)
            {
                float time = (back.Z - ballPosition.Z) / ballVelocity.Z;
                collision = ballPosition + ballVelocity * time;

                if (withinBounds(collision))
                    return collision;
            }

            // If the x plane normal dot product with the ball velocity is negative
            if (Vector3.Dot(-Vector3.UnitX, ballVelocity) < 0)
            {
                float time = (right.X - ballPosition.X) / ballVelocity.X;
                collision = ballPosition + ballVelocity * time;

                if (withinBounds(collision))
                {
                    ballVelocity.X *= -1;
                    return detectCollision(collision, ballVelocity);
                }
            }

            if (Vector3.Dot(Vector3.UnitX, ballVelocity) < 0)
            {
                float time = (left.X - ballPosition.X) / ballVelocity.X;
                collision = ballPosition + ballVelocity * time;

                if (withinBounds(collision))
                {
                    ballVelocity.X *= -1;
                    return detectCollision(collision, ballVelocity);
                }
            }

            // If the y plane normal dot product with the ball velocity is negative
            if (Vector3.Dot(-Vector3.UnitY, ballVelocity) < 0)
            {
                float time = (top.Y - ballPosition.Y) / ballVelocity.Y;
                collision = ballPosition + ballVelocity * time;

                if (withinBounds(collision))
                {
                    ballVelocity.Y *= -1;
                    return detectCollision(collision, ballVelocity);
                }
            }

            if (Vector3.Dot(Vector3.UnitY, ballVelocity) < 0)
            {
                float time = (bottom.Y - ballPosition.Y) / ballVelocity.Y;
                collision = ballPosition + ballVelocity * time;

                if (withinBounds(collision))
                {
                    ballVelocity.Y *= -1;
                    return detectCollision(collision, ballVelocity);
                }
            }
            
            return collision;
        }

        private bool withinBounds(Vector3 collision)
        {
            // Put radius back in
            if (collision.Z > front.Z || collision.Z < back.Z)
                return false;
            if (collision.X > right.X || collision.X < left.X)
                return false;
            if (collision.Y > top.Y || collision.Y < bottom.Y)
                return false;
            return true;
        }

        // Calls drawing method for shape using BasicEffect
        public void callDraw(GraphicsDeviceManager graphics, Matrix view, Matrix projection, Color color)
        {
            world = Matrix.CreateScale(shapeDimensions) * Matrix.CreateTranslation(position);
            DrawShape(graphics, world, view, projection, basicEffect, color);
        }

        // Calls drawing method for shape using Effect
        public void callDraw(GraphicsDeviceManager graphics, Matrix view, Matrix projection, Effect effect, Vector3 cameraPosition,
                             TextureCube texture)
        {
            world = Matrix.CreateScale(shapeDimensions) * Matrix.CreateTranslation(cameraPosition);
            DrawShape(graphics, world, view, projection, effect, cameraPosition, texture);
        }
    }
}
