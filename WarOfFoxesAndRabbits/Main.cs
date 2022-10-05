using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
            this.IsFixedTimeStep = true;//false;
           // this.TargetElapsedTime = TimeSpan.FromSeconds(1d/2d ); //60);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
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
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            GameManager.Update(field);

            // TODO: Add your update logic here

            base.Update(gameTime);
            System.Diagnostics.Debug.WriteLine("update");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            for (int y = 0; y < GameVariables.cellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.cellsVerticallyCount; x++)
                {
                    //   field[x, y].setTexture();

                 //   field[x, y].asdasd();

                    _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameVariables.cellSize, y * GameVariables.cellSize, GameVariables.cellSize, GameVariables.cellSize), field[x,y].Color);

                   // _spriteBatch.Draw(field[x, y].texture2d, field[x, y].Position, field[x,y].Color);
                    
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}