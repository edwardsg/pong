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
    abstract class Shape
    {
		public Effect Effect { get; }

		public Vector3 Position { get; protected set; }
		public Vector3 Scale { get; protected set; }

        public Shape(Effect effect, Vector3 position)
        {
			Scale = Vector3.One;
			Effect = effect;
			Position = position;
        }

		public Shape(Effect effect, Vector3 position, float scale) : this(effect, position)
		{
			Scale = new Vector3(scale, scale, scale);
		}

		public Shape(Effect effect, Vector3 position, Vector3 scale) : this(effect, position)
		{
			Scale = scale;
		}

		public virtual void Draw(Effect effect, Vector3 cameraPosition, Matrix projection)
		{
			
		}

		public virtual void Draw(BasicEffect effect, Vector3 cameraPosition, Matrix projection)
		{
			
		}

		public virtual void Update()
		{

		}
    }
}
