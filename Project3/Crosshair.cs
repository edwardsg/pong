using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3
{
	class Crosshair : Shape
	{
		private VertexBuffer VertexBuffer { get; set; }
		private IndexBuffer IndexBufferH { get; set; }
		private IndexBuffer IndexBufferV { get; set; }

		public Crosshair(GraphicsDevice device, Vector3 position, Vector3 scale) : base(device, position, scale)
		{
			// Vertices for creating crosshair lines
			VertexPositionColor[] vertices = new VertexPositionColor[4]
			{
				new VertexPositionColor(new Vector3(-1, 0, 0), Color.Red),
				new VertexPositionColor(new Vector3(1, 0, 0), Color.Red),

				new VertexPositionColor(new Vector3(0, -1, 0), Color.Red),
				new VertexPositionColor(new Vector3(0, 1, 0), Color.Red)
			};

			VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
			VertexBuffer.SetData<VertexPositionColor>(vertices);

			short[] indicesH = new short[2] { 0, 1 };
			IndexBufferH = new IndexBuffer(GraphicsDevice, typeof(short), indicesH.Length, BufferUsage.WriteOnly);
			IndexBufferH.SetData<short>(indicesH);

			short[] indicesV = new short[2] { 2, 3 };
			IndexBufferV = new IndexBuffer(GraphicsDevice, typeof(short), indicesV.Length, BufferUsage.WriteOnly);
			IndexBufferV.SetData<short>(indicesV);
		}

		public override void Draw(Vector3 cameraPosition, Matrix projection)
		{
			GraphicsDevice.SetVertexBuffer(VertexBuffer);
			Effect.VertexColorEnabled = true;

			// Horizontal
			Effect.World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(new Vector3(0, Position.Y, Position.Z));
			Effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Effect.Projection = projection;
			GraphicsDevice.Indices = IndexBufferH;

			foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 1);
			}

			// Vertical
			Effect.World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(new Vector3(Position.X, 0, Position.Z));
			GraphicsDevice.Indices = IndexBufferV;

			foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 1);
			}
		}
	}
}
