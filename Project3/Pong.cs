using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
		private const int windowWidth = 800;
		private const int windowHeight = 500;

		// Game constants
		private const float ballSpeed = 20;
		private const float humanSpeed = 10;
		private const float aiSpeed = 9;

		private Vector3 boundingBoxScale = new Vector3(10, 10, 20);
		private Vector3 paddleScale = new Vector3(1, 1, .2f);
		private Vector3 helperScale = new Vector3(1, 1, .05f);

		// Camera distance from origin, rotation speed
		private const float cameraDistance = 50;
		private const float cameraRotateSpeed = 0.002f;

		private Vector3 cameraPosition;
		private float cameraYaw = 0;
		private float cameraPitch = 0;
		public float minPitch = -MathHelper.PiOver2 + 0.3f;
		public float maxPitch = MathHelper.PiOver2 - 0.3f;

		// Projection
		public const float viewAngle = .9f;
		public const float nearPlane = .01f;
		public const float farPlane = 500;

		Ball ball;
		SkyBox skyBox;
		Box player1, player2, hitHelper;
		Shape[] shapes;

        private SoundEffect ballBounce;
        private Song backgroundSong;

        // Player position changes
        float player1Y = 0;
        float player1X = 0;
        float player2Y = 0;
        float player2Z = 0;

        VertexBuffer boxVertexBuffer, boundingBoxVertexBuffer, crosshairVertexBuffer;
		IndexBuffer boxIndexBuffer, boundingBoxIndexBuffer, crosshairHIndexBuffer, crosshairVIndexBuffer;

        Effect skyBoxEffect;
        BasicEffect basicEffect;
        BasicEffect boundingBoxEffect;
        TextureCube skyBoxTexture;

        Texture2D player1Texture;
        Texture2D player2Texture;
        Texture2D helperTexture;

		Vector3 ballHitHelper;
        Vector3 ballHitHelperDimensions;

		bool fPressed = false;

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
			Window.Title = "Space Cadet 3D Ping Pong";

			Vector3[] normals = new Vector3[6]
			{
				new Vector3(0, 0, -1),  // Front
                new Vector3(0, 0, 1),   // Back
                new Vector3(-1, 0, 0),  // Right
                new Vector3(1, 0, 0),   // Left
                new Vector3(0, -1, 0),  // Top
                new Vector3(0, 1, 0)    // Bottom
            };

			Vector3[] planes = new Vector3[6]
			{
				new Vector3(0, 0, 20),  // Front
                new Vector3(0, 0, -20),   // Back
                new Vector3(10, 0, 0),  // Right
                new Vector3(-10, 0, 0),   // Left
                new Vector3(0, 10, 0),  // Top
                new Vector3(0, -10, 0)    // Bottom
            };

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
			
            skyBoxEffect = Content.Load<Effect>("skybox");
            skyBoxTexture = Content.Load<TextureCube>("Islands");
            boundingBoxEffect = new BasicEffect(GraphicsDevice);

            player1Texture = Content.Load<Texture2D>("player1Paddle");
            player2Texture = Content.Load<Texture2D>("player2Paddle");
            helperTexture = Content.Load<Texture2D>("shadow");

            ballBounce = Content.Load<SoundEffect>("blip");

            ball = new Ball(GraphicsDevice, Vector3.Zero, Vector3.UnitZ * ballSpeed, Color.White, ballBounce);
			skyBox = new SkyBox(GraphicsDevice, Vector3.Zero, 200, skyBoxTexture, skyBoxEffect);
			player1 = new Box(GraphicsDevice, new Vector3(0, 0, boundingBoxScale.Z), paddleScale, Color.White, player1Texture);
			player2 = new Box(GraphicsDevice, new Vector3(0, 0, -boundingBoxScale.Z), paddleScale, Color.White, player2Texture);
			hitHelper = new Box(GraphicsDevice, new Vector3(0, 0, boundingBoxScale.Z), helperScale, Color.White, helperTexture);
            
            backgroundSong = Content.Load<Song>("kickshock");
            MediaPlayer.Play(backgroundSong);
            MediaPlayer.IsRepeating = true;

            shapes = new Shape[5] { ball, skyBox, player1, player2, hitHelper };

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

			boxVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), cubeVertices.Length, BufferUsage.WriteOnly);
			boxVertexBuffer.SetData<VertexPositionNormalTexture>(cubeVertices);

			short[] cubeIndices = new short[36]
			{
				0, 1, 3, 2, 3, 1,
				4, 5, 7, 6, 7, 5,
				8, 9, 11, 10, 11, 9,
				12, 13, 15, 14, 15, 13,
				16, 17, 19, 18, 19, 17,
				20, 21, 23, 22, 23, 21
			};

            boxIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), cubeIndices.Length, BufferUsage.WriteOnly);
            boxIndexBuffer.SetData<short>(cubeIndices);

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

			VertexPositionColor[] crosshairVertices = new VertexPositionColor[4]
			{
				new VertexPositionColor(new Vector3(-1, 0, 0), Color.Red),
				new VertexPositionColor(new Vector3(1, 0, 0), Color.Red),

				new VertexPositionColor(new Vector3(0, -1, 0), Color.Red),
				new VertexPositionColor(new Vector3(0, 1, 0), Color.Red)
			};

			crosshairVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), crosshairVertices.Length, BufferUsage.WriteOnly);
			crosshairVertexBuffer.SetData<VertexPositionColor>(crosshairVertices);

			short[] crosshairHIndices = new short[2] { 1, 2 };
			crosshairHIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), crosshairHIndices.Length, BufferUsage.WriteOnly);
			crosshairHIndexBuffer.SetData<short>(crosshairHIndices);

			short[] crosshairVIndices = new short[2] { 0, 1 };
			crosshairVIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), crosshairVIndices.Length, BufferUsage.WriteOnly);
			crosshairVIndexBuffer.SetData<short>(crosshairVIndices);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			boxVertexBuffer.Dispose();
            boxIndexBuffer.Dispose();
			boundingBoxVertexBuffer.Dispose();
			boundingBoxIndexBuffer.Dispose();
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

			// Fullscreen mode
			if (keyboard.IsKeyDown(Keys.F))
			{
				if (fPressed == false)
					fPressed = true;

				if (!graphics.IsFullScreen)
				{
					graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				}
				else
				{
					graphics.PreferredBackBufferWidth = windowWidth;
					graphics.PreferredBackBufferHeight = windowHeight;
				}

				graphics.ToggleFullScreen();
			}

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

			Vector3 player1Velocity = Vector3.Zero;

			// Player 1 X movement - Right and Left arrow keys
			if (keyboard.IsKeyDown(Keys.Right) && player1.Position.X < boundingBoxScale.X - player1.Scale.X)
				player1Velocity.X = 1;
			else if (keyboard.IsKeyDown(Keys.Left) && player1.Position.X > -boundingBoxScale.X + player1.Scale.X)
				player1Velocity.X = -1;

			// Player 1 Y movement - Up and Down arrow keys
			if (keyboard.IsKeyDown(Keys.Up) && player1.Position.Y < boundingBoxScale.Y - player1.Scale.Y)
				player1Velocity.Y = 1;
			else if (keyboard.IsKeyDown(Keys.Down) && player1.Position.Y > -boundingBoxScale.Y + player1.Scale.Y)
				player1Velocity.Y = -1;
			
			float timePassed = milliseconds / 1000;

			if (player1Velocity != Vector3.Zero)
				player1Velocity = Vector3.Normalize(player1Velocity);

			player1.Velocity = player1Velocity * humanSpeed;
			player1.Update(timePassed);

			bool hitPaddle = UpdateBall(timePassed);

			if (hitPaddle)
				checkBallBounds();

			if (ball.Velocity.Z < 0)
				updateAI(timePassed);

			base.Update(gameTime);
		}

		private bool UpdateBall(float timePassed)
		{
			ball.Update(timePassed);

			// If ball is at the Z bounds of the box at the side with player 1
			if (ball.Position.Z > boundingBoxScale.Z - Ball.radius)
				return checkPlayer(player1);

			// If ball is at the Z bounds of the box at the side with player 2
			if (ball.Position.Z < -boundingBoxScale.Z + Ball.radius)
				return checkPlayer(player2);

			if (ball.Position.X > boundingBoxScale.X - Ball.radius || ball.Position.X < -boundingBoxScale.X + Ball.radius)
				ball.BounceX();

			if (ball.Position.Y > boundingBoxScale.Y - Ball.radius || ball.Position.Y < -boundingBoxScale.Y + Ball.radius)
				ball.BounceY();

			return false;
		}

		private bool checkPlayer(Box player)
		{
			// If the position of the ball is within the bounds of the position of the paddle
			if (ball.Position.X <= player.Position.X + 1f && ball.Position.X >= player.Position.X - 1f &&
				ball.Position.Y <= player.Position.Y + 1f && ball.Position.Y >= player.Position.Y - 1f)
			{
				float xDifference = ball.Position.X - player.Position.X;
				float yDifference = ball.Position.Y - player.Position.Y;
				Vector3 ballVelocity = Vector3.Normalize(ball.Velocity);

				ballVelocity += new Vector3(xDifference, yDifference, 0);
				ballVelocity.Normalize();
				ballVelocity *= ballSpeed;
				ballVelocity.Z *= -1;
				ball.Velocity = ballVelocity;
				ball.Sound.Play();

				return true;
			}
			else // Else the ball went out of the bounds and should be reset
			{
				ball.Position = Vector3.Zero;
				ball.Velocity = Vector3.UnitZ * ballSpeed;

				hitHelper.Position = new Vector3(0, 0, 20);

				player1.Position = new Vector3(0, 0, player1.Position.Z);
				player2.Position = new Vector3(0, 0, player2.Position.Z);

				return false;
			}
		}

		// Made to check if the ball hits a wall so that we can implement some sort of color
		private void checkBallBounds()
        {
			// I don't think I'm doing the offset right. This is to have the hitHelper be on the bounding
			// box instead of the playing field.
			Vector3 tempPosition = hitHelper.detectCollision(ball.Position, ball.Velocity);
			Vector3 offset = vectorFromSigns(tempPosition);
			tempPosition.Z = offset.Z * 20;

			hitHelper.Position = tempPosition;
		}

		// Used to get the signs of the position vector for the hitHelper to offset it properly
		public Vector3 vectorFromSigns(Vector3 tempPosition)
		{
			Vector3 offset = Vector3.Zero;

			if (tempPosition.X > 0)
				offset.X = 1;
			else if (tempPosition.X < 0)
				offset.X = -1;

			if (tempPosition.Y > 0)
				offset.Y = 1;
			else if (tempPosition.Y < 0)
				offset.Y = -1;

			if (tempPosition.Z > 0)
				offset.Z = 1;
			else if (tempPosition.Z < 0)
				offset.Z = -1;

			return offset;
		}

		private void updateAI(float timePassed)
		{
			// Based on hitHelper position
			Vector3 player2Velocity = Vector3.Zero;
			Vector3 helperPosition = hitHelper.Position;
			if (player2.Position.X < helperPosition.X && player2.Position.X < boundingBoxScale.X - player2.Scale.X)
				player2Velocity.X = 1;
			if (player2.Position.X > helperPosition.X && player2.Position.X > -boundingBoxScale.X + player2.Scale.X)
				player2Velocity.X = -1;

			if (player2.Position.Y < helperPosition.Y && player2.Position.Y < boundingBoxScale.Y - player2.Scale.Y)
				player2Velocity.Y = 1;
			if (player2.Position.Y > helperPosition.Y && player2.Position.Y > -boundingBoxScale.Y + player2.Scale.Y)
				player2Velocity.Y = -1;

			if (player2Velocity != Vector3.Zero)
				player2Velocity = Vector3.Normalize(player2Velocity);

			player2.Velocity = player2Velocity * aiSpeed;
			player2.Update(timePassed);
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
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(viewAngle, GraphicsDevice.Viewport.AspectRatio, nearPlane, farPlane);

			GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set VertexBuffer and IndexBuffer for SkyBox and Paddles

			foreach (Shape shape in shapes)
			{
				GraphicsDevice.SetVertexBuffer(boxVertexBuffer);
				GraphicsDevice.Indices = boxIndexBuffer;

				shape.Draw(cameraPosition, projection);
			}

			boundingBoxEffect.World = Matrix.CreateScale(boundingBoxScale);
			boundingBoxEffect.View = view;
			boundingBoxEffect.Projection = projection;

			//boundingBoxEffect.DiffuseColor = Color.Aquamarine.ToVector3();
			boundingBoxEffect.LightingEnabled = true;
			boundingBoxEffect.DirectionalLight0.Enabled = true;
			boundingBoxEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, 1, 1));
			boundingBoxEffect.DirectionalLight0.DiffuseColor = Color.MediumVioletRed.ToVector3();
			boundingBoxEffect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
			boundingBoxEffect.DirectionalLight1.Enabled = true;
			boundingBoxEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1, -1, -1));
			boundingBoxEffect.DirectionalLight1.DiffuseColor = Color.MediumVioletRed.ToVector3();
			boundingBoxEffect.DirectionalLight1.SpecularColor = Color.White.ToVector3();

			GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

			foreach (EffectPass pass in boundingBoxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
			}

			boundingBoxEffect.World = Matrix.CreateScale(new Vector3(boundingBoxScale.X - .1f, boundingBoxScale.Y - .1f, boundingBoxScale.Z - .1f));

			GraphicsDevice.SetVertexBuffer(boundingBoxVertexBuffer);
			GraphicsDevice.Indices = boundingBoxIndexBuffer;

			boundingBoxEffect.LightingEnabled = false;

			foreach (EffectPass pass in boundingBoxEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 24);
			}

			boundingBoxEffect.World = Matrix.CreateScale(new Vector3(boundingBoxScale.X, 1, 1)) * Matrix.CreateTranslation(new Vector3(0, ball.Position.Y - Ball.radius, ball.Position.Z - Ball.radius));
			GraphicsDevice.Indices = crosshairHIndexBuffer;

			foreach (EffectPass pass in boundingBoxEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 1);
			}

			boundingBoxEffect.World = Matrix.CreateScale(new Vector3(1, boundingBoxScale.Y, 1)) * Matrix.CreateTranslation(new Vector3(ball.Position.X + Ball.radius, 0, ball.Position.Z - Ball.radius));
			GraphicsDevice.Indices = crosshairVIndexBuffer;

			foreach (EffectPass pass in boundingBoxEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 1);
			}

			base.Draw(gameTime);
		}
    }
}
