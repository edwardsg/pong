using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project3
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Pong : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		// Window size
		private const int windowWidth = 800; //1600
		private const int windowHeight = 500; //800

		// Camera distance from origin, rotation speed
		private const float cameraDistance = 50;
		private const float cameraRotateSpeed = 0.002f;

		// Projection
		private const float viewAngle = .9f;
		private const float nearPlane = .01f;
		private const float farPlane = 500;

		private Vector3 cameraPosition;
		private float cameraYaw = 0;
		private float cameraPitch = 0;
		public float minPitch = -MathHelper.PiOver2 + 0.3f;
		public float maxPitch = MathHelper.PiOver2 - 0.3f;

		VertexPositionNormalTexture[] baseCube;
        VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;

        Effect effect;
        BasicEffect baseEffect;
        TextureCube skyboxTexture;

        Matrix paddleWorld;

		public Pong()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Set window position
			int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			Window.Position = new Point(screenWidth / 2 - windowWidth / 2, screenHeight / 2 - windowHeight / 2);

			// Set window size
			graphics.PreferredBackBufferWidth = windowWidth;
			graphics.PreferredBackBufferHeight = windowHeight;
			graphics.ApplyChanges();

			// Set window title
			Window.Title = "Space Cadet 3D Ping Pxong";

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("skybox");
            baseEffect = new BasicEffect(GraphicsDevice);
            skyboxTexture = Content.Load<TextureCube>("SkyBoxTexture");

			// Cube data - four vertices for each face, put into index buffer as 12 triangles
			VertexPositionNormalTexture[] cubeVertices = new VertexPositionNormalTexture[24]
			{
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.UnitX, new Vector2(0, 0)),
				new VertexPositionNormalTexture(new Vector3(1, 1, -1), Vector3.UnitX, new Vector2(1, 0)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), Vector3.UnitX, new Vector2(1, 1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), Vector3.UnitX, new Vector2(0, 1)),

				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), Vector3.UnitY, new Vector2(0, 0)),
				new VertexPositionNormalTexture(new Vector3(1, 1, -1), Vector3.UnitY, new Vector2(1, 0)),
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.UnitY, new Vector2(1, 1)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), Vector3.UnitY, new Vector2(0, 1)),

				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), Vector3.UnitZ, new Vector2(0, 0)),
				new VertexPositionNormalTexture(new Vector3(1, 1, 1), Vector3.UnitZ, new Vector2(1, 0)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), Vector3.UnitZ, new Vector2(1, 1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), Vector3.UnitZ, new Vector2(0, 1)),

				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), -Vector3.UnitX, new Vector2(0, 0)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, 1), -Vector3.UnitX, new Vector2(1, 0)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), -Vector3.UnitX, new Vector2(1, 1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), -Vector3.UnitX, new Vector2(0, 1)),

				new VertexPositionNormalTexture(new Vector3(-1, -1, 1), -Vector3.UnitY, new Vector2(0, 0)),
				new VertexPositionNormalTexture(new Vector3(1, -1, 1), -Vector3.UnitY, new Vector2(1, 0)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), -Vector3.UnitY, new Vector2(1, 1)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), -Vector3.UnitY, new Vector2(0, 1)),

				new VertexPositionNormalTexture(new Vector3(1, 1, -1), -Vector3.UnitZ, new Vector2(0, 0)),
				new VertexPositionNormalTexture(new Vector3(-1, 1, -1), -Vector3.UnitZ, new Vector2(1, 0)),
				new VertexPositionNormalTexture(new Vector3(-1, -1, -1), -Vector3.UnitZ, new Vector2(1, 1)),
				new VertexPositionNormalTexture(new Vector3(1, -1, -1), -Vector3.UnitZ, new Vector2(0, 1))
			};

			vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), cubeVertices.Length, BufferUsage.WriteOnly);
			vertexBuffer.SetData<VertexPositionNormalTexture>(cubeVertices);

			short[] cubeIndices = new short[36]
			{
				0, 1, 3, 2, 3, 1,
				4, 5, 7, 6, 7, 5,
				8, 9, 11, 10, 11, 9,
				12, 13, 15, 14, 15, 13,
				16, 17, 19, 18, 19, 17,
				20, 21, 23, 22, 23, 21
			};

			indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), cubeIndices.Length, BufferUsage.WriteOnly);
			indexBuffer.SetData<short>(cubeIndices);

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			vertexBuffer.Dispose();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			KeyboardState keyboard = Keyboard.GetState();

			if (keyboard.IsKeyDown(Keys.Escape))
				Exit();

			float milliseconds = gameTime.ElapsedGameTime.Milliseconds;

			// Camera Y rotation - A and D
			if (keyboard.IsKeyDown(Keys.A))
				cameraYaw -= cameraRotateSpeed * milliseconds;
			else if (keyboard.IsKeyDown(Keys.D))
				cameraYaw += cameraRotateSpeed * milliseconds;

			// Camera X rotation - W and S
			if (keyboard.IsKeyDown(Keys.W) && cameraPitch > minPitch)
				cameraPitch -= cameraRotateSpeed * milliseconds;
			else if (keyboard.IsKeyDown(Keys.S) && cameraPitch < maxPitch)
				cameraPitch += cameraRotateSpeed * milliseconds;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Rotate camera around origin
			Matrix rotation = Matrix.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
			cameraPosition = Vector3.Transform(Vector3.Backward, rotation);
			cameraPosition *= cameraDistance;

			// Set up scale, camera direction, and perspective projection
			Matrix world = Matrix.CreateScale(100);
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(viewAngle, GraphicsDevice.Viewport.AspectRatio, nearPlane, farPlane);

			GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
			GraphicsDevice.Indices = indexBuffer;

            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                effect.Parameters["World"].SetValue(world * Matrix.CreateTranslation(cameraPosition));
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                effect.Parameters["SkyBoxTexture"].SetValue(skyboxTexture);

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }

			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            paddleWorld = Matrix.CreateScale(new Vector3(2, 2, 0.25f));

            foreach (EffectPass pass in baseEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                baseEffect.World = paddleWorld;
                baseEffect.View = view;
                baseEffect.Projection = projection;
                baseEffect.EnableDefaultLighting();
                baseEffect.DiffuseColor = new Vector3(0, 1, 0);

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }

            base.Draw(gameTime);
		}
	}
}
