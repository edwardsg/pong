using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primitives;
using Microsoft.Xna.Framework.Audio;

namespace Project3
{
    class Ball : Shape
    {
		private new BasicEffect Effect { get; }

		private Vector3 velocity;
		public Vector3 Velocity {
			get { return velocity; }
			set { velocity = value; }
		}

		public Color Color { get; }

        private SpherePrimitive sphere;

        public SoundEffect Sound { get; }

		public float radius = 1;

        public Ball(BasicEffect effect, Vector3 position, Vector3 velocity, Color color, SoundEffect sound) : base(effect, position)
        {
			Effect = effect;
			Color = color;
			this.velocity = velocity;
			sphere = new SpherePrimitive(effect.GraphicsDevice);
            Sound = sound;
        }

		public override void Update(float timePassed)
		{
			Position += velocity * timePassed;
		}

        public void setVelocity(Vector3 update)
        {
            velocity += update;
        }

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

		public override void Draw(BasicEffect effect, Vector3 cameraPosition, Matrix projection)
		{
			effect.World = Matrix.CreateTranslation(Position);
			effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			effect.Projection = projection;
			effect.EnableDefaultLighting();
			effect.DiffuseColor = Color.ToVector3();

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				sphere.Draw(effect);
			}
		}
    }
}
