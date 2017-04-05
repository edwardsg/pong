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
            position += update;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getShapeDimensions()
        {
            return shapeDimensions;
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
