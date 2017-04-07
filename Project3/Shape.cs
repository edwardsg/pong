using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Project3
{
    abstract class Shape
    {
		protected GraphicsDevice GraphicsDevice { get; set; }
		public BasicEffect Effect { get; protected set; }

		public Vector3 Position { get; set; }
		public Vector3 Scale { get; protected set; }

        public Shape(GraphicsDevice device, Vector3 position)
        {
			GraphicsDevice = device;
			Scale = Vector3.One;
			Position = position;
        }

		public Shape(GraphicsDevice device, Vector3 position, Vector3 scale) : this(device, position)
		{
			Scale = scale;
		}

		public virtual void Draw(Vector3 cameraPosition, Matrix projection) {}
		public virtual void Update(float timePassed) {}
    }
}
