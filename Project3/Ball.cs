using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primitives;

namespace Project3
{
    class Ball : Shape
    {
		private new BasicEffect Effect { get; }

		private Vector3 velocity;

		public Color Color { get; }

        private SpherePrimitive sphere;

        public Ball(BasicEffect effect, Vector3 position, Vector3 velocity, Color color) : base(effect, position)
        {
			Effect = effect;
			Color = color;
			this.velocity = velocity;
			sphere = new SpherePrimitive(effect.GraphicsDevice);
        }

        public void setPosition(Vector3 update)
        {
            Position += update;
        }

        public void setVelocity(Vector3 update)
        {
            velocity += update;
        }

        public void UpdateBall(float timePassed, Box player1, Box player2, Matrix boundingBoxWorld)
        {
			Position += velocity * timePassed;
            
            // If ball is at the X bounds of the box at the side with player 1
            if (Position.Z > boundingBoxWorld.M33 - player1.getShapeDimensions().Z)
                checkPlayer(player1.Position);

            // If ball is at the X bounds of the box at the side with player 2
            if (Position.Z < -boundingBoxWorld.M33 + player2.getShapeDimensions().Z)
                checkPlayer(player2.Position);

            if (Position.Y > boundingBoxWorld.M22 - 1 || Position.Y < -boundingBoxWorld.M22 + 1)
                velocity.Y *= -1;

            if (Position.X > boundingBoxWorld.M11 - 1 || Position.X < -boundingBoxWorld.M11 + 1)
                velocity.X *= -1;
        }

        private void checkPlayer(Vector3 playerPosition)
        {
            // If the base.Position of the ball is within the bounds of the base.Position of the paddle
            if (Position.X <= playerPosition.X + 2f && Position.X >= playerPosition.X - 2f &&
                Position.Y <= playerPosition.Y + 2f && Position.Y >= playerPosition.Y - 2f)
            {
                float xDifference = Position.X - playerPosition.X;
                float yDifference = Position.Y - playerPosition.Y;
                float zDifference = Position.Z - playerPosition.Z;
                velocity.Normalize();

                velocity += new Vector3(xDifference, yDifference, zDifference);
                velocity.Normalize();
                velocity *= 1;
                velocity.Z *= -1;
            }

            // Else the ball went out of the bounds and should be reset
            else
            {
                Position = Vector3.Zero;
                velocity = new Vector3(0, 0, 1f);
            }
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
