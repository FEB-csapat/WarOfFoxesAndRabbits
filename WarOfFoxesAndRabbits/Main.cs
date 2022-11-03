using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D rectangleBlock;
        private SpriteFont spriteFont;

        private MouseState previousMouseState;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH,
                PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Window.Title = GameConstants.TITLE;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            spriteFont = Content.Load<SpriteFont>("Fonts/Arial");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ComponentManager.Instance.Initialize(Content);
            GameManager.Instance.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // If escape button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            GameManager.Instance.TickCounter++;
            if (GameManager.Instance.TickCounter % (60 / GameManager.Instance.Tickrate) == 0)
            {
                if (!GameManager.Instance.IsPaused)
                {
                    GameManager.Instance.Update();

                    ComponentManager.Instance.Update();
                }
            }

            MouseState currentMouseState = Mouse.GetState();

            ComponentManager.Instance.UpdateCoordinationLabel(currentMouseState);

            // Mouse clicked
            if (previousMouseState.LeftButton == ButtonState.Released
            && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                ComponentManager.Instance.CheckIfButtonWasClicked(currentMouseState);
            }

            // Mouse being held
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                ComponentManager.Instance.CheckIfCanDraw(currentMouseState);
            }

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Set background color
            GraphicsDevice.Clear(Color.DarkGray);
            spriteBatch.Begin();

            GameManager.Instance.Draw(spriteBatch, rectangleBlock);

            ComponentManager.Instance.Draw(spriteBatch, rectangleBlock, spriteFont);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}