/* CS361 Computer Graphics, Project 3: 3D Pong
 * Shaelyn Divins and Gabriel Edwards
 * Dr. Barry Wittman
 * 2017-04-07
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;

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
		private const float aiSpeed = 5;

		// Winning condition
		private const int winningScore = 3;

		// Object sizes
		private Vector3 boundingBoxScale = new Vector3(10, 10, 20);
		private Vector3 paddleScale = new Vector3(1, 1, .2f);
		private Vector3 helperScale = new Vector3(1, 1, 0);

		// Camera distance from origin, rotation speed
		private const float cameraDistance = 50;
		private const float cameraRotateSpeed = 0.002f;

		// Limit vertical rotation
		public float minPitch = -MathHelper.PiOver2 + 0.3f;
		public float maxPitch = MathHelper.PiOver2 - 0.3f;

		// Other camera variables
		private Vector3 cameraPosition;
		private float cameraYaw = 0;
		private float cameraPitch = 0;

		// Projection
		public const float viewAngle = .9f;
		public const float nearPlane = .01f;
		public const float farPlane = 500;

		// Game objects
		private SkyBox skyBox;
		private BoundingBox boundingBox;
		private Ball ball;
		private Crosshair crosshair;
		private Paddle player1, player2, hitHelper;
		private Shape[] shapes;

		// Game sounds
        private SoundEffect ballBounce;
        private Song backgroundSong;
        private Song winSong;
        private Song loseSong;

		// Scoreboard things
        private SpriteFont scoreFont;
        private SpriteFont conditionFont;
        int player1Score = 0;
        int player2Score = 0;

        bool gamePaused = false;

		// Skybox
        Effect skyBoxEffect;
        TextureCube skyBoxTexture;

		// Paddle textures
        Texture2D player1Texture;
        Texture2D player2Texture;
        Texture2D helperTexture;

		// Fullscreen
		private bool fPressed = false;

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

            Song winSong = Content.Load<Song>("moveforward");
            Song loseSong = Content.Load<Song>("sendforthehorses");

			// BGM
            Song backgroundSong = Content.Load<Song>("kickshock");
            MediaPlayer.Play(backgroundSong);
            MediaPlayer.IsRepeating = true;

			// Create game objects
            skyBoxEffect = Content.Load<Effect>("skybox");
            skyBoxTexture = Content.Load<TextureCube>("Islands");

            player1Texture = Content.Load<Texture2D>("player1WhiteGradient");
            player2Texture = Content.Load<Texture2D>("player2WhiteGradient");
            helperTexture = Content.Load<Texture2D>("shadow");
            scoreFont = Content.Load<SpriteFont>("Arial16");
            conditionFont = Content.Load<SpriteFont>("Arial30");

            ballBounce = Content.Load<SoundEffect>("blip");

			skyBox = new SkyBox(GraphicsDevice, Vector3.Zero, 200, skyBoxTexture, skyBoxEffect);
			boundingBox = new BoundingBox(GraphicsDevice, Vector3.Zero, boundingBoxScale, Color.MediumVioletRed);
            ball = new Ball(GraphicsDevice, Vector3.Zero, Vector3.UnitZ * ballSpeed, Color.Blue, ballBounce);
			crosshair = new Crosshair(GraphicsDevice, ball.Position, boundingBoxScale, Color.White);
			player1 = new Paddle(GraphicsDevice, new Vector3(0, 0, boundingBoxScale.Z), paddleScale, Color.Yellow, player1Texture);
			player2 = new Paddle(GraphicsDevice, new Vector3(0, 0, -boundingBoxScale.Z), paddleScale, Color.Coral, player2Texture);
			hitHelper = new Paddle(GraphicsDevice, new Vector3(0, 0, boundingBoxScale.Z + .1f), helperScale, Color.Green, helperTexture);

			// Single array for all objects
            shapes = new Shape[7] { skyBox, boundingBox, crosshair, ball, player1, player2, hitHelper };			
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{

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

            // Fullscreen mode
            if (keyboard.IsKeyDown(Keys.F))
            {
                if (fPressed == false)
                    fPressed = true;

				// Change resolution depnding on mode
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

            if (!gamePaused)
            {
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

				// Bounce ball off paddles
                if (hitPaddle)
                    checkBallBounds();

				// Move AI if ball moving towards it
                if (ball.Velocity.Z < 0)
                    updateAI(timePassed);

				// Crosshair follows ball
				crosshair.Position = ball.Position;
            }

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                gamePaused = false;
                player1Score = player2Score = 0;
                MediaPlayer.Stop();
                MediaPlayer.Play(backgroundSong);
            }
            
			base.Update(gameTime);
		}

		// Move ball and bounce off walls, increase scores
		private bool UpdateBall(float timePassed)
		{
			ball.Update(timePassed);

            // If ball is at the Z bounds of the box at the side with player 1
            if (ball.Position.Z > boundingBoxScale.Z - Ball.radius)
            {
                bool hit = collidePaddle(player1);
                if (!hit)
                    player2Score += 1;
                return hit;
            }

			// If ball is at the Z bounds of the box at the side with player 2
			if (ball.Position.Z < -boundingBoxScale.Z + Ball.radius)
            {
                bool hit = collidePaddle(player2);
                if (!hit)
                    player1Score += 1;
                return hit;
            }

			// Wall bounces
            if (ball.Position.X > boundingBoxScale.X - Ball.radius || ball.Position.X < -boundingBoxScale.X + Ball.radius)
				ball.BounceX();

			if (ball.Position.Y > boundingBoxScale.Y - Ball.radius || ball.Position.Y < -boundingBoxScale.Y + Ball.radius)
				ball.BounceY();

			return false;
		}

		// Check ball against paddle and bounce
		private bool collidePaddle(Box player)
		{
			// If the position of the ball is within the bounds of the position of the paddle
			if (ball.Position.X <= player.Position.X + 1f && ball.Position.X >= player.Position.X - 1f &&
				ball.Position.Y <= player.Position.Y + 1f && ball.Position.Y >= player.Position.Y - 1f)
			{
				float xDifference = ball.Position.X - player.Position.X;
				float yDifference = ball.Position.Y - player.Position.Y;
				Vector3 ballVelocity = Vector3.Normalize(ball.Velocity);

				// Change ball's velocity depending on collision point
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
				// Reset ball
				ball.Position = Vector3.Zero;
				ball.Velocity = Vector3.UnitZ * ballSpeed;

				// Reset helper
				hitHelper.Position = new Vector3(0, 0, boundingBoxScale.Z + .1f);

				// Reset paddles
				player1.Position = new Vector3(0, 0, player1.Position.Z);
				player2.Position = new Vector3(0, 0, player2.Position.Z);

				return false;
			}
		}

		// Made to check if the ball hits a wall so that we can implement some sort of color
		private void checkBallBounds()
        {
			Vector3 tempPosition = hitHelper.detectCollision(ball.Position, ball.Velocity);
			Vector3 offset = vectorFromSigns(tempPosition);
			tempPosition.Z = offset.Z * boundingBoxScale.Z + .1f;

            Vector3.Clamp(tempPosition, -boundingBoxScale, boundingBoxScale);

			hitHelper.Position = tempPosition;
		}

		// Used to get the signs of the position vector for the hitHelper to offset it properly
		private Vector3 vectorFromSigns(Vector3 tempPosition)
		{
			Vector3 offset = Vector3.Zero;

			if (tempPosition.Z > 0)
				offset.Z = 1;
			else if (tempPosition.Z < 0)
				offset.Z = -1;

			return offset;
		}

        // Used to update position of AI
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
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Rotate camera around origin
            Matrix rotation = Matrix.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
			cameraPosition = Vector3.Transform(Vector3.Backward * 1f, rotation); //was 1.5f, but I changed it for debugging purposes
			cameraPosition *= cameraDistance;

			// Set up camera direction, and perspective projection
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(viewAngle, GraphicsDevice.Viewport.AspectRatio, nearPlane, farPlane);

			GraphicsDevice.Clear(Color.CornflowerBlue);

			// Make sure spot texture has transparent background
			GraphicsDevice.BlendState = BlendState.AlphaBlend;

			// Render all game objects
			foreach (Shape shape in shapes)
				shape.Draw(cameraPosition, projection);
            
            // Draw the scores for the human and computer players
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            var score = new StringBuilder();
            
            score.Append("Human Score: ");
            score.Append(player1Score).AppendLine();
            spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(16, 16), Color.White);

            score.Clear();
            score.Append("Computer Score: ");
            score.Append(player2Score).AppendLine();
            spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(GraphicsDevice.Viewport.Width - scoreFont.MeasureString(score).X - 16, 16), Color.White);

            checkWin();

            spriteBatch.End();

            base.Draw(gameTime);
		}

		// Check if one player won and display text
        public void checkWin()
        {
            if (player1Score == winningScore)
            {
                string win = "You Win!";
                spriteBatch.DrawString(conditionFont, win, new Vector2((GraphicsDevice.Viewport.Width / 2) - scoreFont.MeasureString(win).X, (GraphicsDevice.Viewport.Height / 2) - scoreFont.MeasureString(win).Y), Color.White);
                if (!gamePaused)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(winSong);
                }
                gamePaused = true;
            }

            if (player2Score == winningScore)
            {
                string lose = "You Lose!";
                spriteBatch.DrawString(conditionFont, lose, new Vector2((GraphicsDevice.Viewport.Width / 2) - scoreFont.MeasureString(lose).X, (GraphicsDevice.Viewport.Height / 2) - scoreFont.MeasureString(lose).Y), Color.White);
                if (!gamePaused)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(loseSong);
                }
                gamePaused = true;
            }
        }
    }
}
