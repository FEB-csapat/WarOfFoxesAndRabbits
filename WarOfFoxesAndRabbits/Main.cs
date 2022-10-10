using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D rectangleBlock;
        SpriteFont spriteFont;


        Cell[,] field = new Cell[GameVariables.CellsHorizontallyCount, GameVariables.CellsVerticallyCount];

        MouseState previousMouseState;

        bool paused = false;
        bool enabledLakes = false;

        // Variables for components
        readonly List<Component> components = new List<Component>();
        Label generationLabel;
        int generation = 0;
        Label foxLabel;
        Label rabbitLabel;

        PencilType pencilSelected = PencilType.NONE;

        static long tickCounter = 0;
        static int tickrate = 1;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GameVariables.WindowWidth;
            _graphics.PreferredBackBufferHeight = GameVariables.WindowHeight;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Window.Title = GameVariables.Title;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            #region Components

            spriteFont = Content.Load<SpriteFont>("Fonts/Arial");

            // TODO: Generation counter resets with clear button
            generationLabel = new Label("Generation: " + generation, new Vector2(GameVariables.GetGameCanvasWidth() + 50, 4));
            components.Add(generationLabel);

            // Button to regenerate the field
            Button regenerateButton = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 30), () =>
            {
                generateField();
            }, text: "Regenerate", width: 150, height: 50);
            components.Add(regenerateButton);

            // Button to enable/disable lake generation
            Button lakeSwitchButton = new Button();
            lakeSwitchButton = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 210, 30), () =>
            {
                enabledLakes = !enabledLakes;
                if (enabledLakes)
                {
                    lakeSwitchButton.text = "Disable lake";
                }
                else
                {
                    lakeSwitchButton.text = "Enable lake";
                }
            }, text: "Enable lake", width: 150, height: 50);
            components.Add(lakeSwitchButton);

            // Button to pause the game
            Button pauseButton = new Button();
            pauseButton = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 90), () =>
            {
                paused = !paused;
                if (paused) pauseButton.text = "Continue";
                else pauseButton.text = "Pause";
            }, text: "Pause", width: 150, height: 50);
            components.Add(pauseButton);

            // Label to show the tickrate
            Label tickrateLabel = new Label(tickrate.ToString() + " tick", new Vector2(GameVariables.GetGameCanvasWidth() + 100, 166));
            components.Add(tickrateLabel);

            // Increment tickrate button
            components.Add(new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 150, 150), () =>
            {
                if (tickrate < 59)
                {
                    tickrate += 2;
                    tickrateLabel.text = tickrate.ToString() + " tick";
                }
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 150), () =>
            {
                if (tickrate > 1)
                {
                    tickrate -= 2;
                    tickrateLabel.text = tickrate.ToString() + " tick";
                }
            }, text: "-", width: 50, height: 50));

            // Button to draw animals on the field

            Button rabbitPencil = new Button();
            Button foxPencil = new Button();
            Button wallPencil = new Button();
            Button waterPencil = new Button();
            Button grassPencil = new Button();
            rabbitPencil = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 210), () =>
            {
                pencilSelected = PencilType.BUNNY;
                rabbitPencil.isSelected = true;
                foxPencil.isSelected = false;
                wallPencil.isSelected = false;
                waterPencil.isSelected = false;
                grassPencil.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50);
            components.Add(rabbitPencil);

            // Fox pencil
            foxPencil = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 270), () =>
            {

                pencilSelected = PencilType.FOX;
                rabbitPencil.isSelected = false;
                foxPencil.isSelected = true;
                wallPencil.isSelected = false;
                waterPencil.isSelected = false;
                grassPencil.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Fox"), width: 50, height: 50);
            components.Add(foxPencil);

            // Wall pencil
            wallPencil = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 110, 210), () =>
            {
                pencilSelected = PencilType.WALL;
                rabbitPencil.isSelected = false;
                foxPencil.isSelected = false;
                wallPencil.isSelected = true;
                waterPencil.isSelected = false;
                grassPencil.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Wall"), width: 50, height: 50);
            components.Add(wallPencil);

            // Water pencil
            waterPencil = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 110, 270), () =>
            {
                pencilSelected = PencilType.WATER;
                rabbitPencil.isSelected = false;
                foxPencil.isSelected = false;
                wallPencil.isSelected = false;
                waterPencil.isSelected = true;
                grassPencil.isSelected = false;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Water"), width: 50, height: 50);
            components.Add(waterPencil);


            // Grass pencil
            grassPencil = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 170, 210), () =>
            {
                pencilSelected = PencilType.GRASS;
                rabbitPencil.isSelected = false;
                foxPencil.isSelected = false;
                wallPencil.isSelected = false;
                waterPencil.isSelected = false;
                grassPencil.isSelected = true;
                // Icon from https://icons8.com
            }, image: Content.Load<Texture2D>("Images/Grass"), width: 50, height: 50);
            components.Add(grassPencil);

            // Button to clear the fields from animals
            Button clearButton = new Button(new Vector2(GameVariables.GetGameCanvasWidth() + 50, 385), () =>
            {
                for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
                {
                    for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                    {
                        if (field[x, y].animal is Animal)
                        {
                            field[x, y].animal = null;
                        }

                    }
                }
            }, text: "Clear", width: 150, height: 50);
            components.Add(clearButton);



            // Label to count foxes and rabbits
            foxLabel = new Label("Number of rabbits: " + 0, new Vector2(GameVariables.GetGameCanvasWidth() + 50, 330));
            components.Add(foxLabel);
            rabbitLabel = new Label("Number of rabbits: " + 0, new Vector2(GameVariables.GetGameCanvasWidth() + 50, 350));
            components.Add(rabbitLabel);

            #endregion

            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            generateField();
        }

        protected void generateField()
        {
            FastNoiseLite noise = new FastNoiseLite();
            if (enabledLakes)
            {
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                noise.SetSeed(GameVariables.Random.Next(0, 1000000));
            }

            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                {
                    bool hasWater = false;

                    if (enabledLakes && noise.GetNoise(x, y) >= 0.55)
                    {
                        hasWater = true;
                    }

                    field[x, y] = new Cell(x, y, matter: hasWater ? new Water() : null);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            tickCounter++;
            if (tickCounter % (60 / tickrate) == 0)
            {
                if (!paused)
                {
                    GameManager.Update(field);
                    generation++;
                }
            }

            // Calculations for labels
            if (field.CountAnimals() == 0)
            {
                generation = 0;
            }
            else
            {
                generationLabel.text = "Generation: " + generation;
            }

            rabbitLabel.text = "Rabbits: " + field.CountRabbits();
            foxLabel.text = "Foxes: " + field.CountFoxes();


            MouseState currentMouseState = Mouse.GetState();

            // Mouse clicked
            if (previousMouseState.LeftButton == ButtonState.Released
            && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Component component in components)
                {
                    if (component.GetType() == typeof(Button))
                    {
                        if (currentMouseState.X >= ((Button)component).position.X && currentMouseState.X <= ((Button)component).position.X + ((Button)component).width
                        && currentMouseState.Y >= ((Button)component).position.Y && currentMouseState.Y <= ((Button)component).position.Y + ((Button)component).height)
                        {
                            ((Button)component).onClick();
                        }
                    }
                }
            }

            // Mouse being held
            if (pencilSelected != PencilType.NONE)

            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
                    {
                        for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                        {
                            if (currentMouseState.X >= field[x, y].posX * GameVariables.CellSize && currentMouseState.X <= field[x, y].posX * GameVariables.CellSize + GameVariables.CellSize
                            && currentMouseState.Y >= field[x, y].posY * GameVariables.CellSize && currentMouseState.Y <= field[x, y].posY * GameVariables.CellSize + GameVariables.CellSize
                            )
                            {
                                if (field[x, y].animal == null)
                                {
                                    switch (pencilSelected)
                                    {
                                        case PencilType.BUNNY:
                                            field[x, y].animal = new Rabbit();
                                            break;
                                        case PencilType.FOX:
                                            field[x, y].animal = new Fox();
                                            break;
                                        case PencilType.WALL:
                                            field[x, y].matter = new Wall();
                                            break;
                                        case PencilType.WATER:
                                            field[x, y].matter = new Water();
                                            break;
                                        case PencilType.GRASS:
                                            field[x, y].matter = new Grass();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            _spriteBatch.Begin();
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsVerticallyCount; x++)
                {
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(x * GameVariables.CellSize, y * GameVariables.CellSize, GameVariables.CellSize, GameVariables.CellSize), field[x, y].Color);
                }
            }

            foreach (Component component in components)
            {
                if (component.GetType() == typeof(Button))
                {
                    if (((Button)component).image != null)
                    {

                        if (((Button)component).isSelected)
                        {
                            _spriteBatch.Draw(rectangleBlock, new Rectangle(((int)((Button)component).position.X) - 4, ((int)((Button)component).position.Y) - 4, ((Button)component).width + 6, ((Button)component).height + 6), Color.Black);
                        }
                        _spriteBatch.Draw(((Button)component).image, ((Button)component).position, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(rectangleBlock, new Rectangle((int)((Button)component).position.X, (int)((Button)component).position.Y, ((Button)component).width, ((Button)component).height), Color.Gray);
                    }

                    _spriteBatch.DrawString(spriteFont, ((Button)component).text, new Vector2(((Button)component).position.X + ((Button)component).width / 4, ((Button)component).position.Y + ((Button)component).height / 3), Color.Black);
                }
                else if (component.GetType() == typeof(Label))
                {
                    _spriteBatch.DrawString(spriteFont, ((Label)component).text, new Vector2(component.position.X, component.position.Y), Color.Black);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}