﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Primitives;
using Microsoft.Xna.Framework.Audio;

namespace Project3
{
	// A sphere with velocity and color - also plays sound
    class Ball : Shape
    {
		private Vector3 velocity;
		public Vector3 Velocity {
			get { return velocity; }
			set { velocity = value; }
		}

		public Color Color { get; }

        private SpherePrimitive sphere;

        public SoundEffect Sound { get; }

		public const float radius = 1;

        public Ball(GraphicsDevice device, Vector3 position, Vector3 velocity, Color color, SoundEffect sound) : base(device, position)
        {
			Color = color;
			this.velocity = velocity;
			sphere = new SpherePrimitive(GraphicsDevice);
            Sound = sound;
			Effect = new BasicEffect(GraphicsDevice);
        }

		// Move ball along its velocity vector
		public override void Update(float timePassed)
		{
			Position += velocity * timePassed;
		}

		// Bounce ball by reversing vectors in appropriate direction
		public void BounceX()
		{
			velocity.X *= -1;
            Sound.Play();
		}

		public void BounceY()
		{
			velocity.Y *= -1;
            Sound.Play();
		}

		public override void Draw(Vector3 cameraPosition, Matrix projection)
		{
			// Draw sphere primitive using BasicEFfect
			Effect.World = Matrix.CreateTranslation(Position);
			Effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Effect.Projection = projection;
			Effect.EnableDefaultLighting();
			Effect.DiffuseColor = Color.ToVector3();

			foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				sphere.Draw(Effect);
			}
		}
    }
}
