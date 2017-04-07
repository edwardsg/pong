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
		private const int windowWidth = 800; //1600
		private const int windowHeight = 500; //800

		// Camera distance from origin, rotation speed
		private const float cameraDistance = 50;
		private const float cameraRotateSpeed = 0.002f;

		// Projection
		public const float viewAngle = .9f;
		public const float nearPlane = .01f;
		public const float farPlane = 500;

		private Vector3 cameraPosition;
		private float cameraYaw = 0;
		private float cameraPitch = 0;
		public float minPitch = -MathHelper.PiOver2 + 0.3f;
		public float maxPitch = MathHelper.PiOver2 - 0.3f;

		private Vector3 boundingBoxScale = new Vector3(10, 10, 20);

        private SoundEffect ballBounce;
        private Song backgroundSong;

        // Player position changes
        float player1Y = 0;
        float player1X = 0;
        float player2Y = 0;
        float player2Z = 0;

		Ball ball;
		SkyBox skyBox;
		Box player1, player2, hitHelper;
		Shape[] shapes;
        //Box boundingBoxShape;

        VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;

        VertexBuffer boundingBoxVertexBuffer;
        IndexBuffer boundingBoxIndexBuffer;

        Effect skyBoxEffect;
        BasicEffect basicEffect;
        BasicEffect boundingBoxEffect;
        TextureCube skyBoxTexture;

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

            basicEffect = new BasicEffect(GraphicsDevice);
            skyBoxEffect = Content.Load<Effect>("skybox");
            skyBoxTexture = Content.Load<TextureCube>("Islands");
            boundingBoxEffect = new BasicEffect(GraphicsDevice);

            ballBounce = Content.Load<SoundEffect>("blip");

            ball = new Ball(basicEffect, Vector3.Zero, Vector3.UnitZ, Color.Purple, ballBounce);
			skyBox = new SkyBox(skyBoxEffect, Vector3.Zero, 200, skyBoxTexture);
			player1 = new Box(basicEffect, new Vector3(0, 0, 20), new Vector3(1, 1, 0.2f), Color.Green);
			player2 = new Box(basicEffect, new Vector3(0, 0, -20), new Vector3(1, 1, 0.2f), Color.Yellow);
			hitHelper = new Box(basicEffect, new Vector3(0, 0, 20), new Vector3(1, 1, 0.001f), Color.Red);
            
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
			if (keyboard.IsKeyDown(Keys.Up) && player1.Position.Y < boundingBoxScale.Y - player1.Scale.Y)
				player1Y += 0.2f;
			if (keyboard.IsKeyDown(Keys.Down) && player1.Position.Y > -boundingBoxScale.Y + player1.Scale.Y)
				player1Y -= 0.2f;

			// Player 1 Z movement - Right and Left arrow keys
			if (keyboard.IsKeyDown(Keys.Right) && player1.Position.X < boundingBoxScale.X - player1.Scale.X)
				player1X += 0.2f;
			if (keyboard.IsKeyDown(Keys.Left) && player1.Position.X > -boundingBoxScale.X + player1.Scale.X)
				player1X -= 0.2f;

			// Temporary fix; need to limit to arrow keys
			if (keyboard.GetPressedKeys().Length > 1)
			{
				player1Y *= MathHelper.ToRadians(45);
				player1X *= MathHelper.ToRadians(45);
			}
			
			player1.Update(player1.Position + new Vector3(player1X, player1Y, 0));
			float timePassed = gameTime.ElapsedGameTime.Milliseconds / 100f;
			bool hitPaddle = UpdateBall(timePassed);

			if (hitPaddle)
				checkBallBounds();

			if (ball.Velocity.Z < 0)
				updateAI();

			base.Update(gameTime);
		}

		private bool UpdateBall(float timePassed)
		{
			Vector3 position = ball.Position + ball.Velocity * timePassed;

			ball.Update(position);

			// If ball is at the Z bounds of the box at the side with player 1
			if (position.Z > boundingBoxScale.Z - ball.radius)
				return ball.checkPlayer(player1.Position, hitHelper);

			// If ball is at the Z bounds of the box at the side with player 2
			if (position.Z < -boundingBoxScale.Z + ball.radius)
				return ball.checkPlayer(player2.Position, hitHelper);

			if (position.X > boundingBoxScale.X - ball.radius || position.X < -boundingBoxScale.X + ball.radius)
				ball.BounceX();

			if (position.Y > boundingBoxScale.Y - ball.radius || position.Y < -boundingBoxScale.Y + ball.radius)
				ball.BounceY();

			return false;
		}

		// Made to check if the ball hits a wall so that we can implement some sort of color
		private void checkBallBounds()
        {
			// I don't think I'm doing the offset right. This is to have the hitHelper be on the bounding
			// box instead of the playing field.
			Vector3 tempPosition = hitHelper.detectCollision(ball.Position, ball.Velocity);
			Vector3 offset = vectorFromSigns(tempPosition);
			tempPosition.Z = offset.Z * 20;

			hitHelper.Update(tempPosition);
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

		private void updateAI()
		{
			// Based on hitHelper position
			float movement = 0.1f;

			Vector3 playerPosition = player2.Position;
			Vector3 helperPosition = hitHelper.Position;
			if (playerPosition.X < helperPosition.X)
				playerPosition.X += movement;
			if (playerPosition.X > helperPosition.X)
				playerPosition.X -= movement;

			if (playerPosition.Y < helperPosition.Y)
				playerPosition.Y += movement;
			if (playerPosition.Y > helperPosition.Y)
				playerPosition.Y -= movement;

			player2.Update(playerPosition);
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
				GraphicsDevice.SetVertexBuffer(vertexBuffer);
				GraphicsDevice.Indices = indexBuffer;

				if (shape.Effect is BasicEffect)
					shape.Draw((BasicEffect) shape.Effect, cameraPosition, projection);
				else
					shape.Draw(shape.Effect, cameraPosition, projection);
			}

			GraphicsDevice.SetVertexBuffer(boundingBoxVertexBuffer);
            GraphicsDevice.Indices = boundingBoxIndexBuffer;

			Matrix boundingBoxWorld = Matrix.CreateScale(boundingBoxScale);

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
