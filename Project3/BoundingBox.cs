using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3
{
	class BoundingBox : Box
	{
		private VertexBuffer linesVertexBuffer;
		private IndexBuffer linesIndexBuffer;

		public BoundingBox(GraphicsDevice device, Vector3 position, Vector3 scale) : base(device, position, scale)
		{
			// Vertices for creating lines of bounding box
			VertexPosition[] boundingBox = new VertexPosition[8]
			{
				new VertexPosition(new Vector3(-1, -1, 1)),
				new VertexPosition(new Vector3(-1, 1, 1)),
				new VertexPosition(new Vector3(1, 1, 1)),
				new VertexPosition(new Vector3(1, -1, 1)),

				new VertexPosition(new Vector3(1, -1, -1)),
				new VertexPosition(new Vector3(-1, -1, -1)),
				new VertexPosition(new Vector3(-1, 1, -1)),
				new VertexPosition(new Vector3(1, 1, -1)),
			};

			linesVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPosition), boundingBox.Length, BufferUsage.WriteOnly);
			linesVertexBuffer.SetData<VertexPosition>(boundingBox);

			short[] boundingBoxIndices = new short[48]
			{
				0, 1, 1, 2, 2, 3, 3, 0,
				3, 4, 4, 7, 7, 2, 2, 3,
				4, 5, 5, 6, 6, 7, 7, 4,
				5, 0, 0, 1, 1, 6, 6, 5,
				1, 2, 2, 7, 7, 6, 6, 1,
				0, 3, 3, 4, 4, 5, 5, 0
			};

			linesIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), boundingBoxIndices.Length, BufferUsage.WriteOnly);
			linesIndexBuffer.SetData<short>(boundingBoxIndices);
		}

		public override void Draw(Vector3 cameraPosition, Matrix projection)
		{
			Effect.World = Matrix.CreateScale(Scale);
			Effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Effect.Projection = projection;

			// Two directional lights
			Effect.VertexColorEnabled = false;
			Effect.LightingEnabled = true;
			Effect.DirectionalLight0.Enabled = true;
			Effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 1, 1));
			Effect.DirectionalLight0.DiffuseColor = Color.MediumVioletRed.ToVector3();
			Effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
			Effect.DirectionalLight1.Enabled = true;
			Effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1, -1, -1));
			Effect.DirectionalLight1.DiffuseColor = Color.MediumVioletRed.ToVector3();
			Effect.DirectionalLight1.SpecularColor = Color.White.ToVector3();

			// To only see background faces
			GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

			GraphicsDevice.SetVertexBuffer(VertexBuffer);
			GraphicsDevice.Indices = IndexBuffer;

			foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
			}

			// Bounding box lines
			GraphicsDevice.SetVertexBuffer(linesVertexBuffer);
			GraphicsDevice.Indices = linesIndexBuffer;

			Effect.World = Matrix.CreateScale(new Vector3(Scale.X - .1f, Scale.Y - .1f, Scale.Z - .1f));
			Effect.LightingEnabled = false;

			foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 24);
			}
		}
	}
}
