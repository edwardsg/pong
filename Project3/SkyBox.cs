using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project3
{
    class SkyBox : Shape
    {
		private TextureCube texture;

		public SkyBox(Effect effect, Vector3 position, int scale, TextureCube texture) : base(effect, position, scale)
		{
			this.texture = texture;
		}

		public override void Draw(Effect effect, Vector3 cameraPosition, Matrix projection)
		{
			GraphicsDevice device = effect.GraphicsDevice;

			Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

			effect.Parameters["World"].SetValue(world);
			effect.Parameters["View"].SetValue(view);
			effect.Parameters["Projection"].SetValue(projection);
			effect.Parameters["CameraPosition"].SetValue(Position);
			effect.Parameters["SkyBoxTexture"].SetValue(texture);

			device.RasterizerState = RasterizerState.CullClockwise;

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
			}
		}
	}
}
