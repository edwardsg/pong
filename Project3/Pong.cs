using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Primitives;

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

        Matrix world;
        Matrix view;
        Matrix projection;

        // Player position changes
        float player1Y = 0;
        float player1X = 0;
        float player2Y = 0;
        float player2Z = 0;

        Ball ball;
        Box player1;
        Box player2;
        Box skybox;
        Box boundingBoxShape;

        VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;

        VertexBuffer boundingBoxVertexBuffer;
        IndexBuffer boundingBoxIndexBuffer;

        Effect effect;
        BasicEffect ballEffect;
        BasicEffect boundingBoxEffect;
        TextureCube skyboxTexture;
        
        Matrix boundingBoxWorld;
        
        Vector3 ballHitHelper;
        Vector3 ballHitHelperDimensions;

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

            ball = new Ball(graphics, new Vector3(0, 0, 0), new Vector3(0, 0, 1f));
            player1 = new Box(graphics, new Vector3(0, 0, 20), new Vector3(1, 1, 0.2f));
            player2 = new Box(graphics, new Vector3(0, 0, -20), new Vector3(1, 1, 0.2f));
            skybox = new Box(graphics, new Vector3(0, 0, 0), new Vector3(200, 200, 200));

            boundingBoxWorld = Matrix.CreateScale(new Vector3(10, 10, 20));

            ballHitHelper = new Vector3(-20f, 10f, 0);
            ballHitHelperDimensions = new Vector3(0.001f, 2, 2);

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
            ballEffect = new BasicEffect(GraphicsDevice);
            boundingBoxEffect = new BasicEffect(GraphicsDevice);
            skyboxTexture = Content.Load<TextureCube>("Islands");

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

            boundingBoxVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPosition), boundingBox.Length, BufferUsage.WriteOnly);
            boundingBoxVertexBuffer.SetData<VertexPosition>(boundingBox);

            short[] boundingBoxIndices = new short[48]
            {
                0, 1, 1, 2, 2, 3, 3, 0,
                3, 4, 4, 7, 7, 2, 2, 3,
                4, 5, 5, 6, 6, 7, 7, 4,
                5, 0, 0, 1, 1, 6, 6, 5,
                1, 2, 2, 7, 7, 6, 6, 1,
                0, 3, 3, 4, 4, 5, 5, 0
            };

            boundingBoxIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), boundingBoxIndices.Length, BufferUsage.WriteOnly);
            boundingBoxIndexBuffer.SetData<short>(boundingBoxIndices);

        }

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			vertexBuffer.Dispose();
            indexBuffer.Dispose();
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

            player1Y = 0;
            player1X = 0;

            // Player 1 Y movement - Up and Down arrow keys
            if (keyboard.IsKeyDown(Keys.Up) && player1.getPosition().Y < boundingBoxWorld.M22 - player1.getShapeDimensions().Y)
                player1Y += 0.3f;
            if (keyboard.IsKeyDown(Keys.Down) && player1.getPosition().Y > -boundingBoxWorld.M22 + player1.getShapeDimensions().Y)
                player1Y -= 0.3f;

            // Player 1 Z movement - Right and Left arrow keys
            if (keyboard.IsKeyDown(Keys.Right) && player1.getPosition().X < boundingBoxWorld.M11 - player1.getShapeDimensions().X)
                player1X += 0.3f;
            if (keyboard.IsKeyDown(Keys.Left) && player1.getPosition().X > -boundingBoxWorld.M11 + player1.getShapeDimensions().X)
                player1X -= 0.3f;

            // Temporary fix
            if (keyboard.GetPressedKeys().Length > 1)
            {
                player1Y *= MathHelper.ToRadians(45);
                player1X *= MathHelper.ToRadians(45);
            }

            player1.setPosition(new Vector3(player1X, player1Y, 0));
            float timePassed = gameTime.ElapsedGameTime.Milliseconds / 100f;
            ball.UpdateBall(timePassed, player1, player2, boundingBoxWorld);

            base.Update(gameTime);
		}

        // Made to check if the ball hits a wall so that we can implement some sort of color
        private void checkBallBounds()
        {
            
        }

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Rotate camera around origin
			Matrix rotation = Matrix.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
			cameraPosition = Vector3.Transform(Vector3.Backward * 1f, rotation); //was 1.5f, but I changed it for debugging purposes
			cameraPosition *= cameraDistance;

			// Set up scale, camera direction, and perspective projection
			world = Matrix.CreateScale(200);
			view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			projection = Matrix.CreatePerspectiveFieldOfView(viewAngle, GraphicsDevice.Viewport.AspectRatio, nearPlane, farPlane);

			GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set VertexBuffer and IndexBuffer for SkyBox and Paddles
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
			GraphicsDevice.Indices = indexBuffer;

            // Draw SkyBox
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            skybox.callDraw(graphics, view, projection, effect, cameraPosition, skyboxTexture);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            
            // Draw paddles
            player1.callDraw(graphics, view, projection, Color.Green);
            player2.callDraw(graphics, view, projection, Color.Yellow);

            // Draw ball
            ball.callDraw(graphics, view, projection, Color.Purple);



            
            GraphicsDevice.SetVertexBuffer(boundingBoxVertexBuffer);
            GraphicsDevice.Indices = boundingBoxIndexBuffer;

            //boundingBoxWorld = Matrix.CreateScale(20); //Matrix.CreateScale(new Vector3(20, 10, 10));
            boundingBoxEffect.DiffuseColor = Color.White.ToVector3();

            foreach (EffectPass pass in boundingBoxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                boundingBoxEffect.World = boundingBoxWorld;
                boundingBoxEffect.View = view;
                boundingBoxEffect.Projection = projection;

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 24);
            }
            
            base.Draw(gameTime);
		}
    }
}
