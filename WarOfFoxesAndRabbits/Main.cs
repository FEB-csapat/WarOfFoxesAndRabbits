using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D rectangleBlock;
        private SpriteFont spriteFont;


        private MouseState previousMouseState;

        #region Variables for components

        private readonly List<Component> components = new();
        private Label generationLabel;

        private Label foxLabel;
        private Label rabbitLabel;

        private Label foxDeathLabel;
        private Label rabbitDeathLabel;
        private Label coordLabel;

        private Graph graph;

        private PencilType pencilSelected = PencilType.NONE;

        private PencilSizeType pencilSizeSelected = PencilSizeType.SMALL;

        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GameConstants.WindowWidth,
                PreferredBackBufferHeight = GameConstants.WindowHeight
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Window.Title = GameConstants.Title;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            #region Initializing components

            spriteFont = Content.Load<SpriteFont>("Fonts/Arial");

            // TODO: Generation counter resets with clear button
            generationLabel = new Label("Generation: " + GameManager.Instance.Generation, new Vector2(GameConstants.GameCanvasWidth + 50, 4));
            components.Add(generationLabel);

            // Button to regenerate the field
            Button regenerateButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 30), () =>
            {
                GameManager.Instance.GenerateField();
            }, text: "Regenerate", width: 150, height: 50);
            components.Add(regenerateButton);

            // Button to enable/disable lake generation
            Button lakeSwitchButton = new Button();
            lakeSwitchButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 210, 30), () =>
            {
                GameManager.Instance.IsLakeEnabled = !GameManager.Instance.IsLakeEnabled;
                if (GameManager.Instance.IsLakeEnabled)
                {
                    lakeSwitchButton.Text = "Disable lake";
                }
                else
                {
                    lakeSwitchButton.Text = "Enable lake";
                }
            }, text: "Enable lake", width: 150, height: 50);
            components.Add(lakeSwitchButton);

            // Button to pause the game
            Button pauseButton = new Button();
            pauseButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 90), () =>
            {
                GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
                if (GameManager.Instance.IsPaused)
                {
                    pauseButton.Text = "Continue";
                }
                else
                {
                    pauseButton.Text = "Pause";
                }
            }, text: "Pause", width: 150, height: 50);
            components.Add(pauseButton);

            // Label to show the tickrate
            Label tickrateLabel = new Label(GameManager.Instance.Tickrate.ToString() + " tick", new Vector2(GameConstants.GameCanvasWidth + 102, 166));
            components.Add(tickrateLabel);


            // Label to show the fox death counter
            foxDeathLabel = new Label("Fox death: " + GameManager.Instance.FoxDeathCounter, new Vector2(GameConstants.GameCanvasWidth + 50, 650));
            components.Add(foxDeathLabel);

            // Label to show the rabbit death counter
            rabbitDeathLabel = new Label("Rabbit death: " + GameManager.Instance.RabbitDeathCounter, new Vector2(GameConstants.GameCanvasWidth + 50, 700));
            components.Add(rabbitDeathLabel);

            // Small brush button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 210), () =>
            {
                pencilSizeSelected = PencilSizeType.SMALL;
            }, image: Content.Load<Texture2D>("Images/Pencil1"), width: 50, height: 50));

            // Medium brush button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 270), () =>
            {
                pencilSizeSelected = PencilSizeType.MEDIUM;
            }, image: Content.Load<Texture2D>("Images/Pencil2"), width: 50, height: 50));

            // Large brush button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 330), () =>
            {
                pencilSizeSelected = PencilSizeType.LARGE;
            }, image: Content.Load<Texture2D>("Images/Pencil3"), width: 50, height: 50));

            // Increment tickrate button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 160, 150), () =>
            {
                GameManager.Instance.IncrementTickrate();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 150), () =>
            {
                GameManager.Instance.DecrementTickrate();

                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "-", width: 50, height: 50));

            //Min tickrate
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 220, 150), () =>
            {
                GameManager.Instance.SetTickrateToMinimum();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "Min", width: 50, height: 50));
            // Max tickrate
            components.Add(new Button(new Vector2(GameConstants.GameCanvasWidth + 280, 150), () =>
            {
                GameManager.Instance.SetTickrateToMaximum();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "Max", width: 50, height: 50));


            // Button to draw animals on the field
            Button rabbitPencil = new();
            Button foxPencil = new();
            Button wallPencil = new();
            Button waterPencil = new();
            Button grassPencil = new();
            rabbitPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 210), () =>
            {
                pencilSelected = PencilType.BUNNY;
                rabbitPencil.IsSelected = true;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50);
            components.Add(rabbitPencil);

            // Fox pencil
            foxPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 270), () =>
            {

                pencilSelected = PencilType.FOX;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = true;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Fox"), width: 50, height: 50);
            components.Add(foxPencil);

            // Wall pencil
            wallPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 110, 210), () =>
            {
                pencilSelected = PencilType.WALL;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = true;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Wall"), width: 50, height: 50);
            components.Add(wallPencil);

            // Water pencil
            waterPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 110, 270), () =>
            {
                pencilSelected = PencilType.WATER;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = true;
                grassPencil.IsSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Water"), width: 50, height: 50);
            components.Add(waterPencil);


            // Grass pencil
            grassPencil = new Button(new Vector2(GameConstants.GameCanvasWidth + 170, 210), () =>
            {
                pencilSelected = PencilType.GRASS;
                rabbitPencil.IsSelected = false;
                foxPencil.IsSelected = false;
                wallPencil.IsSelected = false;
                waterPencil.IsSelected = false;
                grassPencil.IsSelected = true;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Grass"), width: 50, height: 50);
            components.Add(grassPencil);

            // Button to clear the fields from animals
            Button clearButton = new Button(new Vector2(GameConstants.GameCanvasWidth + 50, 385), () =>
            {
                GameManager.Instance.ClearAnimals();
                GameManager.Instance.ResetDeathCounters();
                GameManager.Instance.ResetGeneration();
                generationLabel.Text = "Generation: " + GameManager.Instance.Generation;
            }, text: "Clear", width: 150, height: 50);
            components.Add(clearButton);

            // Label to count foxes and rabbits
            foxLabel = new Label("Foxes: " + 0, new Vector2(GameConstants.GameCanvasWidth + 50, 330));
            components.Add(foxLabel);
            rabbitLabel = new Label("Rabbits: " + 0, new Vector2(GameConstants.GameCanvasWidth + 50, 350));
            components.Add(rabbitLabel);

            coordLabel = new Label($"(x: ,y:)", new Vector2(0, 0));

            graph = new Graph(new Vector2(GameConstants.GameCanvasWidth + 50, 450));

            #endregion

            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameManager.Instance.Initialize();
        }



        protected override void Update(GameTime gameTime)
        {
            // If escape button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            GameManager.Instance.TickCounter++;
            if (GameManager.Instance.TickCounter % (60 / GameManager.Instance.Tickrate) == 0)
            {
                if (!GameManager.Instance.IsPaused)
                {
                    GameManager.Instance.Update();

                    #region Updating components

                    rabbitLabel.Text = "Rabbits: " + GameManager.Instance.RabbitCounter;
                    foxLabel.Text = "Foxes: " + GameManager.Instance.FoxCounter;

                    rabbitDeathLabel.Text = "Rabbit deaths: " + GameManager.Instance.RabbitDeathCounter;
                    foxDeathLabel.Text = "Fox deaths: " + GameManager.Instance.FoxDeathCounter;

                    GameManager.Instance.IncrementGeneration();
                    if (GameManager.Instance.RabbitCounter + GameManager.Instance.FoxCounter == 0)
                    {
                        GameManager.Instance.ResetGeneration();
                    }
                    else
                    {
                        generationLabel.Text = "Generation: " + GameManager.Instance.Generation;
                    }

                    if (GameManager.Instance.RabbitCounter > 0)
                    {
                        graph.AddData(AnimalType.RABBIT, GameManager.Instance.RabbitCounter);

                    }
                    if (GameManager.Instance.FoxCounter > 0)
                    {
                        graph.AddData(AnimalType.FOX, GameManager.Instance.FoxCounter);
                    }

                    graph.Update();

                    #endregion
                }
            }

            MouseState currentMouseState = Mouse.GetState();

            #region Update coordination label

            coordLabel.Position = new Vector2(currentMouseState.X + 10, currentMouseState.Y + 10);
            coordLabel.Text = $"(x:{1 + currentMouseState.X / GameConstants.CellSize} ,y:{1 + currentMouseState.Y / GameConstants.CellSize})";

            #endregion

            #region Mouse clicked

            if (previousMouseState.LeftButton == ButtonState.Released
            && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Component component in components)
                {
                    if (component is Button button)
                    {
                        if (currentMouseState.X >= button.Position.X && currentMouseState.X <= button.Position.X + button.Width
                        && currentMouseState.Y >= button.Position.Y && currentMouseState.Y <= button.Position.Y + button.Height)
                        {
                            button.OnClick();
                        }
                    }
                }
            }

            #endregion


            void DrawWithPencilAt(int x, int y)
            {
                int size = (int)pencilSizeSelected;
                for (int py = -size; py <= size; py++)
                {
                    for (int px = -size; px <= size; px++)
                    {
                        if (y + py >= 0 && x + px >= 0
                        && y + py < GameConstants.CellsVerticallyCount && x + px < GameConstants.CellsHorizontallyCount
                        && GameManager.Instance.GetFieldCell(x, y).Animal == null)
                        {
                            switch (pencilSelected)
                            {
                                case PencilType.BUNNY:
                                    GameManager.Instance.SetFieldCellAnimal(x + px, y + py, new Rabbit());
                                    break;
                                case PencilType.FOX:
                                    GameManager.Instance.SetFieldCellAnimal(x + px, y + py, new Fox());
                                    break;
                                case PencilType.WALL:
                                    GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Wall());
                                    break;
                                case PencilType.WATER:
                                    GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Water(GameConstants.minWaterDepth));
                                    break;
                                case PencilType.GRASS:
                                    GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Grass());
                                    break;
                            }
                        }
                    }
                }
            }

            #region Mouse being held

            if (currentMouseState.LeftButton == ButtonState.Pressed && pencilSelected != PencilType.NONE)
            {
                for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameConstants.CellsVerticallyCount; x++)
                    {
                        if (currentMouseState.X >= x * GameConstants.CellSize && currentMouseState.X <= x * GameConstants.CellSize + GameConstants.CellSize
                        && currentMouseState.Y >= y * GameConstants.CellSize && currentMouseState.Y <= y * GameConstants.CellSize + GameConstants.CellSize
                        )
                        {
                            DrawWithPencilAt(x, y);
                        }
                    }
                }
            }

            #endregion

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Set background color
            GraphicsDevice.Clear(Color.DarkGray);
            _spriteBatch.Begin();

            #region Draw cells

            for (int y = 0; y < GameConstants.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameConstants.CellsVerticallyCount; x++)
                {
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameConstants.CellSize, y * GameConstants.CellSize, GameConstants.CellSize, GameConstants.CellSize), GameManager.Instance.GetFieldCell(x, y).Color);
                }
            }

            #endregion

            #region Draw components

            foreach (Component component in components)
            {
                if (component is Button button)
                {
                    if (button.ImageTexture != null)
                    {
                        // Draw black "border" to selected button
                        if (button.IsSelected)
                        {
                            _spriteBatch.Draw(rectangleBlock, new Rectangle(((int)button.Position.X) - 4, ((int)button.Position.Y) - 4, button.Width + 6, button.Height + 6), Color.Black);
                        }
                        _spriteBatch.Draw(button.ImageTexture, button.Position, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(rectangleBlock, new Rectangle((int)button.Position.X, (int)button.Position.Y, button.Width, button.Height), Color.Gray);
                    }
                    // Draw text of button
                    _spriteBatch.DrawString(spriteFont, button.Text, new Vector2(button.Position.X + button.Width / 4, button.Position.Y + button.Height / 3), Color.Black);
                }
                else if (component is Label label)
                {
                    _spriteBatch.DrawString(spriteFont, label.Text, new Vector2(component.Position.X, component.Position.Y), Color.Black);
                }
            }

            #endregion

            #region Draw graph

            // Draw background
            _spriteBatch.Draw(rectangleBlock, new Rectangle((int)graph.Position.X, (int)graph.Position.Y, graph.Width, graph.Height), Color.DimGray);

            foreach (GraphData d in graph.Datas)
            {
                _spriteBatch.Draw(rectangleBlock, new Rectangle((int)d.Position.X, (int)d.Position.Y, GameConstants.GraphRectSize, GameConstants.GraphRectSize), d.Color);
            }

            #endregion

            #region Draw coordinates

            if (coordLabel.Position.X - 10 <= GameConstants.GameCanvasWidth
                && coordLabel.Position.X - 10 >= 0
                && coordLabel.Position.Y - 10 >= 0
                && coordLabel.Position.Y - 10 <= GameConstants.GameCanvasWidth)
            {
                _spriteBatch.DrawString(spriteFont, coordLabel.Text, new Vector2(coordLabel.Position.X, coordLabel.Position.Y), Color.Black);
            }

            #endregion

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}