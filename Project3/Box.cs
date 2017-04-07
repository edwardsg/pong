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
		private Vector3 velocity;
		public Vector3 Velocity {
			get { return velocity; }
			set { velocity = value; }
		}

		public Color Color { get; }
        public Texture2D Texture { get; }

        float alphaChange; // For visibility of paddle when in front of the ball

        Vector3 front = new Vector3(0, 0, 20);
        Vector3 back = new Vector3(0, 0, -20);
        Vector3 right = new Vector3(10, 0, 0);
        Vector3 left = new Vector3(-10, 0, 0);
        Vector3 top = new Vector3(0, 10, 0);
        Vector3 bottom = new Vector3(0, -10, 0);

        public Box(GraphicsDevice device, Vector3 position, Vector3 scale, Color color, Texture2D texture) : base(device, position, scale)
        {
			velocity = Vector3.Zero;
			Color = color;
			Effect = new BasicEffect(GraphicsDevice);
            Texture = texture;
		}

		public override void Update(float timePassed)
		{
			Position += velocity * timePassed;
		}

		// To do recursive computations for the position of the AI
		public Vector3 detectCollision(Vector3 ballPosition, Vector3 ballVelocity)
        {
			Vector3 collision = Vector3.Zero;

			// Case where need to check if ball hits plane
			float temp = Vector3.Dot(-Vector3.UnitZ, ballVelocity);
			if (Vector3.Dot(-Vector3.UnitZ, ballVelocity) < 0)
			{
				float time = (front.Z - ballPosition.Z - Ball.radius) / ballVelocity.Z;
				collision = ballPosition + ballVelocity * time;

				if (withinBounds(collision))
					return collision;
			}

			if (Vector3.Dot(Vector3.UnitZ, ballVelocity) < 0)
			{
				float time = (back.Z - ballPosition.Z + Ball.radius) / ballVelocity.Z;
				collision = ballPosition + ballVelocity * time;

				if (withinBounds(collision))
					return collision;
			}

			// If the x plane normal dot product with the ball velocity is negative
			if (Vector3.Dot(-Vector3.UnitX, ballVelocity) < 0)
			{
				float time = (right.X - ballPosition.X - Ball.radius) / ballVelocity.X;
				collision = ballPosition + ballVelocity * time;

				if (withinBounds(collision))
				{
					ballVelocity.X *= -1;
					return detectCollision(collision, ballVelocity);
				}
			}

			if (Vector3.Dot(Vector3.UnitX, ballVelocity) < 0)
			{
				float time = (left.X - ballPosition.X + Ball.radius) / ballVelocity.X;
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
				float time = (top.Y - ballPosition.Y - Ball.radius) / ballVelocity.Y;
				collision = ballPosition + ballVelocity * time;

				if (withinBounds(collision))
				{
					ballVelocity.Y *= -1;
					return detectCollision(collision, ballVelocity);
				}
			}

			if (Vector3.Dot(Vector3.UnitY, ballVelocity) < 0)
			{
				float time = (bottom.Y - ballPosition.Y + Ball.radius) / ballVelocity.Y;
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

		public override void Draw(Vector3 cameraPosition, Matrix projection)
		{
			Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

			Effect.World = world;
			Effect.View = view;
			Effect.Projection = projection;
			Effect.EnableDefaultLighting();
			Effect.DiffuseColor = Color.ToVector3();
            Effect.Texture = Texture;
            Effect.TextureEnabled = true;

			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
			}

            GraphicsDevice.BlendState = BlendState.Opaque;

            Effect.TextureEnabled = false;
        }
	}
}
