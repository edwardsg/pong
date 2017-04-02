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

        // Player position changes
        float player1Y = 0;
        float player1Z = 0;
        float player2Y = 0;
        float player2Z = 0;

		VertexPositionNormalTexture[] baseCube;
        VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;

        /*VertexBuffer boundingBoxVertexBuffer;
        IndexBuffer boundingBoxIndexBuffer;
        VertexPosition[] boundingBox;
        short[] boundingBoxIndices;*/

        Effect effect;
        BasicEffect cubeEffect;
        BasicEffect ballEffect;
        TextureCube skyboxTexture;

        Matrix paddleWorld;
        Matrix ballWorld;
        //Matrix boundingBoxWorld;

        // Player positions
        Vector3 player1Position;
        Vector3 player2Position;

        SpherePrimitive ball;
        Vector3 ballPosition = new Vector3(0, 0, 0);
        Vector3 ballVelocity = new Vector3(-1f, 0, 0);

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

            player1Position = new Vector3(-20, 0, 0);
            player2Position = new Vector3(20, 0, 0);

            ball = new SpherePrimitive(GraphicsDevice);

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
            cubeEffect = new BasicEffect(GraphicsDevice);
            ballEffect = new BasicEffect(GraphicsDevice);
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

            /*boundingBox = new VertexPosition[8]
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

            boundingBoxIndices = new short[48]
            {
                1, 2, 2, 3, 3, 4, 4, 1,
                4, 5, 5, 8, 8, 3, 3, 4,
                5, 6, 6, 7, 7, 8, 8, 5,
                6, 1, 1, 2, 2, 7, 7, 6,
                2, 3, 3, 8, 8, 7, 7, 2,
                1, 4, 4, 5, 5, 6, 6, 1
            };

            boundingBoxIndices = new short[8]
            {
                1, 2, 2, 3, 3, 4, 4, 1
            };

            boundingBoxIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), boundingBoxIndices.Length, BufferUsage.WriteOnly);
            boundingBoxIndexBuffer.SetData<short>(boundingBoxIndices);*/

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

            player1Y = 0;
            player1Z = 0;

            // Player 1 Y movement - Up and Down arrow keys
            if (keyboard.IsKeyDown(Keys.Up))
                player1Y += 0.5f;
            if (keyboard.IsKeyDown(Keys.Down))
                player1Y -= 0.5f;

            // Player 1 Z movement - Right and Left arrow keys
            if (keyboard.IsKeyDown(Keys.Right))
                player1Z += 0.5f;
            if (keyboard.IsKeyDown(Keys.Left))
                player1Z -= 0.5f;

            player1Position += new Vector3(0, player1Y, player1Z);
            float timePassed = gameTime.ElapsedGameTime.Milliseconds / 100f;
            UpdateBall(timePassed);

            base.Update(gameTime);
		}

        private void UpdateBall(float timePassed)
        {
            ballPosition += ballVelocity * timePassed;

            // Preliminarily hardcoded in the bounds for the ball movement; 19.5 is the box and the 1 accounts for the radius of the ball (TODO fix later)

            // If ball is at the X bounds of the box at the side with player 1
            if (ballPosition.X < -19.5f + 1)
                checkPlayer(player1Position);

            // If ball is at the X bounds of the box at the side with player 2
            if (ballPosition.X > 19f - 1)
                checkPlayer(player2Position);
            
            if (ballPosition.Y > 19.5f || ballPosition.Y < -19.5f)
                ballVelocity.Y *= -1;

            if (ballPosition.Z > 19.5f || ballPosition.Z < -19.5f)
                ballVelocity.Z *= -1;
        }

        private void checkPlayer(Vector3 playerPosition)
        {
            // If the position of the ball is within the bounds of the position of the paddle
            if (ballPosition.Z <= playerPosition.Z + 2 && ballPosition.Z >= playerPosition.Z - 2 &&
                ballPosition.Y <= playerPosition.Y + 2 && ballPosition.Y >= playerPosition.Y - 2)
            {
                float xDifference = ballPosition.X - playerPosition.X;
                float yDifference = ballPosition.Y - playerPosition.Y;
                ballVelocity.Normalize();

                ballVelocity += new Vector3(xDifference, yDifference, 0);
                ballVelocity.Normalize();
                ballVelocity *= 1;
            }

            // Else the ball went out of the bounds and should be reset
            else
            {
                ballPosition = Vector3.Zero;
                ballVelocity = new Vector3(-1f, 0, 0);
            }
        }

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Rotate camera around origin
			Matrix rotation = Matrix.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
			cameraPosition = Vector3.Transform(Vector3.Backward * 1.5f, rotation);
			cameraPosition *= cameraDistance;

			// Set up scale, camera direction, and perspective projection
			Matrix world = Matrix.CreateScale(200);
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

            paddleWorld = Matrix.CreateScale(new Vector3(0.25f, 2, 2)) * Matrix.CreateTranslation(player1Position);

            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                cubeEffect.World = paddleWorld;
                cubeEffect.View = view;
                cubeEffect.Projection = projection;
                cubeEffect.EnableDefaultLighting();
                cubeEffect.DiffuseColor = new Vector3(0, 1, 0);

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }

            paddleWorld = Matrix.CreateScale(new Vector3(0.25f, 2, 2)) * Matrix.CreateTranslation(player2Position);

            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                cubeEffect.World = paddleWorld;
                cubeEffect.View = view;
                cubeEffect.Projection = projection;
                cubeEffect.EnableDefaultLighting();
                cubeEffect.DiffuseColor = new Vector3(1, 1, 0);

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }

            ballWorld = Matrix.CreateScale(2);

            foreach (EffectPass pass in ballEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                ballEffect.World = ballWorld * Matrix.CreateTranslation(ballPosition);
                ballEffect.View = view;
                ballEffect.Projection = projection;
                ballEffect.EnableDefaultLighting();
                ballEffect.DiffuseColor = new Vector3(1, 0, 1);

                ball.Draw(ballEffect);
            }

            base.Draw(gameTime);
		}
	}
}
