using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3
{
    class Shape
    {
        public Shape()
        {
        }

        public void DrawShape(GraphicsDeviceManager graphics, Matrix world, Matrix view, Matrix projection, 
                              BasicEffect basicEffect, Color color)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                basicEffect.World = world;
                basicEffect.View = view;
                basicEffect.Projection = projection;
                basicEffect.EnableDefaultLighting();
                basicEffect.DiffuseColor = color.ToVector3();

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }
        }

        public void DrawShape(GraphicsDeviceManager graphics, Matrix world, Matrix view, Matrix projection,
                              Effect effect, Vector3 cameraPosition, TextureCube texture)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                effect.Parameters["World"].SetValue(world * Matrix.CreateTranslation(cameraPosition));
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                effect.Parameters["SkyBoxTexture"].SetValue(texture);

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }
        }

        public void DrawShape(GraphicsDeviceManager graphics, Matrix world, Matrix view, Matrix projection,
                              BasicEffect basicEffect, Color color, SpherePrimitive sphere)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                basicEffect.World = world;
                basicEffect.View = view;
                basicEffect.Projection = projection;
                basicEffect.EnableDefaultLighting();
                basicEffect.DiffuseColor = color.ToVector3();

                sphere.Draw(basicEffect);
            }
        }
    }
}
