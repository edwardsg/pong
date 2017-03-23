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

        VertexPositionNormalTexture[] baseCube;
        VertexBuffer vertexBuffer;

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
			// TODO: Add your initialization logic here

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

            //calculated normals for the cubes
            Vector3 frontNormal = new Vector3(0, 0, 1);
            Vector3 backNormal = new Vector3(0, 0, -1);
            Vector3 topNormal = new Vector3(0, 1, 0);
            Vector3 bottomNormal = new Vector3(0, -1, 0);
            Vector3 leftNormal = new Vector3(-1, 0, 0);
            Vector3 rightNormal = new Vector3(1, 0, 0);

            baseCube = new VertexPositionNormalTexture[36] {
                //front face
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, 1f), frontNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, 1f), frontNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, 1f), frontNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, 1f), frontNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, 1f), frontNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, 1f), frontNormal, new Vector2(1, 1)),

                //back face
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, -1f), backNormal, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, -1f), backNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, -1f), backNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, -1f), backNormal, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, -1f), backNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, -1f), backNormal, new Vector2(1, 0)),

                //top face
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, 1f), topNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, -1f), topNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, -1f), topNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, 1f), topNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, -1f), topNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, 1f), topNormal, new Vector2(1, 1)),

                //bottom face
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, -1f), bottomNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, 1f), bottomNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, 1f), bottomNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, -1f), bottomNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, 1f), bottomNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, -1f), bottomNormal, new Vector2(1, 1)),

                //left face
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, -1f), leftNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, -1f), leftNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, 1f), leftNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, -1f), leftNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(-1f, 1f, 1f), leftNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(-1f, -1f, 1f), leftNormal, new Vector2(1, 1)),

                //right face
                new VertexPositionNormalTexture(new Vector3(1f, -1f, 1f), rightNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, 1f), rightNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, -1f), rightNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, 1f), rightNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(1f, 1f, -1f), rightNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(1f, -1f, -1f), rightNormal, new Vector2(1, 1)),
            };

            //fill vertex buffer for the sky box
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), 36, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(baseCube);

            // TODO: use this.Content to load your game content here
        }

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
