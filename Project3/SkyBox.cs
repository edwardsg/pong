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
		public new Effect Effect { get; private set; }

		private TextureCube texture;

		public SkyBox(GraphicsDevice device, Vector3 position, int scale, TextureCube texture, Effect effect) : base(device, position, scale)
		{
			this.texture = texture;
			Effect = effect;
		}

		public override void Draw(Vector3 cameraPosition, Matrix projection)
		{
			Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

			Effect.Parameters["World"].SetValue(world);
			Effect.Parameters["View"].SetValue(view);
			Effect.Parameters["Projection"].SetValue(projection);
			Effect.Parameters["CameraPosition"].SetValue(Position);
			Effect.Parameters["SkyBoxTexture"].SetValue(texture);

			GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

			foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
			}
		}
	}
}
