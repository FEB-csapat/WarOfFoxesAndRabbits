using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D rectangleBlock;

        Cell[,] field = new Cell[GameVariables.cellsHorizontallyCount, GameVariables.cellsVerticallyCount];

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            // this two line limits the fps to 30
            //   this.IsFixedTimeStep = true;//false;
            //   this.TargetElapsedTime = TimeSpan.FromSeconds(1d/2d ); //60);
        }

        protected override void Initialize()
        {
            Window.Title = GameVariables.title;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                {
                    field[x, y] = new Cell(new Vector2(x * GameVariables.cellSize, y * GameVariables.cellSize), GraphicsDevice);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameTime.TotalGameTime.TotalSeconds >= 1)
            {
                GameManager.Update(field);
            }

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                {
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameVariables.cellSize, y * GameVariables.cellSize, GameVariables.cellSize, GameVariables.cellSize), field[x, y].Color);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}