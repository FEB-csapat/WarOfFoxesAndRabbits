using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    public sealed class ComponentManager
    {
        #region Singleton
        private static ComponentManager instance = null;

        private ComponentManager() { }

        public static ComponentManager Instance
        {
            get
            {
                instance ??= new ComponentManager();
                return instance;
            }
        }
        #endregion

        #region Variables

        private readonly List<Panel> panels = new();
        private readonly List<Button> pencilTypeButtons = new();
        private readonly List<Button> pencilSizeButtons = new();
        private readonly List<Component> components = new();

        public List<Component> GetAllButtons()
        {

            List<Component> allButtons = new();
            allButtons.AddRange(pencilTypeButtons);
            allButtons.AddRange(pencilSizeButtons);
            allButtons.AddRange(components.Where(x => x is Button).ToList());

            return allButtons;
        }

        private Label generationLabel;

        private Label foxLabel;
        private Label rabbitLabel;

        private Label foxDeathLabel;
        private Label rabbitDeathLabel;
        private CoordinationLabel coordinationLabel;

        private Graph graph;

        private PencilType pencilSelected = PencilType.NONE;

        private PencilSizeType pencilSizeSelected = PencilSizeType.SMALL;

        #endregion

        public void Initialize(ContentManager content)
        {
            generationLabel = new Label("Generation: " + GameManager.Instance.Generation,
                new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 4));
            components.Add(generationLabel);

            // Button to regenerate the field
            Button regenerateButton = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 30), () =>
            {
                GameManager.Instance.GenerateField();
            }, text: "Regenerate", width: 150, height: 50);
            components.Add(regenerateButton);

            // Button to enable/disable lake generation
            Button lakeSwitchButton = new Button();
            lakeSwitchButton = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 210, 30), () =>
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
            pauseButton = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 90), () =>
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
            Label tickrateLabel = new Label(GameManager.Instance.Tickrate.ToString() + " tick",
                new Vector2(GameConstants.GAME_CANVAS_WIDTH + 102, 166));
            components.Add(tickrateLabel);


            // Label to show the fox death counter
            foxDeathLabel = new Label("Fox death: " + GameManager.Instance.FoxDeathCounter,
                new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 740));
            components.Add(foxDeathLabel);

            // Label to show the rabbit death counter
            rabbitDeathLabel = new Label("Rabbit death: " + GameManager.Instance.RabbitDeathCounter,
                new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 760));
            components.Add(rabbitDeathLabel);


            // Increment tickrate button
            components.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 160, 150), () =>
            {
                GameManager.Instance.IncrementTickrate();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "+", width: 50, height: 50));

            // Decrement tickrate button
            components.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 150), () =>
            {
                GameManager.Instance.DecrementTickrate();

                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "-", width: 50, height: 50));

            //Min tickrate
            components.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 220, 150), () =>
            {
                GameManager.Instance.SetTickrateToMinimum();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "Min", width: 50, height: 50));

            // Max tickrate
            components.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 280, 150), () =>
            {
                GameManager.Instance.SetTickrateToMaximum();
                tickrateLabel.Text = GameManager.Instance.Tickrate.ToString() + " tick";
            }, text: "Max", width: 50, height: 50));


            // Button to clear the fields from animals
            Button clearButton = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 210), () =>
            {
                GameManager.Instance.ClearAnimals();
                GameManager.Instance.ResetDeathCounters();
                GameManager.Instance.ResetGeneration();
                generationLabel.Text = "Generation: " + GameManager.Instance.Generation;
            }, text: "Clear", width: 150, height: 50);
            components.Add(clearButton);


            // Button to draw animals on the field

            Button rabbitPencil = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 280), () =>
            {
                pencilSelected = PencilType.BUNNY;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Rabbit"), width: 50, height: 50, id: PencilType.BUNNY);
            pencilTypeButtons.Add(rabbitPencil);

            // Fox pencil
            Button foxPencil = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 340), () =>
            {

                pencilSelected = PencilType.FOX;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Fox"), width: 50, height: 50, id: PencilType.FOX);
            pencilTypeButtons.Add(foxPencil);

            // Wall pencil
            Button wallPencil = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 110, 280), () =>
            {
                pencilSelected = PencilType.WALL;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Wall"), width: 50, height: 50, id: PencilType.WALL);
            pencilTypeButtons.Add(wallPencil);

            // Water pencil
            Button waterPencil = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 110, 340), () =>
            {
                pencilSelected = PencilType.WATER;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Water"), width: 50, height: 50, id: PencilType.WATER);
            pencilTypeButtons.Add(waterPencil);


            // Grass pencil
            Button grassPencil = new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 170, 280), () =>
            {
                pencilSelected = PencilType.GRASS;
                // Icon from https://icons8.com
            }, image: content.Load<Texture2D>("Images/Grass"), width: 50, height: 50, id: PencilType.GRASS);
            pencilTypeButtons.Add(grassPencil);



            // Small brush button
            pencilSizeButtons.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 280, 280), () =>
            {
                pencilSizeSelected = PencilSizeType.SMALL;
            }, image: content.Load<Texture2D>("Images/Pencil1"), width: 50, height: 50, id: PencilSizeType.SMALL));

            // Medium brush button
            pencilSizeButtons.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 280, 340), () =>
            {
                pencilSizeSelected = PencilSizeType.MEDIUM;
            }, image: content.Load<Texture2D>("Images/Pencil2"), width: 50, height: 50, id: PencilSizeType.MEDIUM));

            // Large brush button
            pencilSizeButtons.Add(new Button(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 280, 400), () =>
            {
                pencilSizeSelected = PencilSizeType.LARGE;
            }, image: content.Load<Texture2D>("Images/Pencil3"), width: 50, height: 50, id: PencilSizeType.LARGE));


            // Label to count foxes and rabbits
            foxLabel = new Label("Foxes: " + 0, new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 470));
            components.Add(foxLabel);
            rabbitLabel = new Label("Rabbits: " + 0, new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 490));
            components.Add(rabbitLabel);


            graph = new Graph(new Vector2(GameConstants.GAME_CANVAS_WIDTH + 50, 530));

            Panel pencilPanel = new Panel(
                new Vector2(GameConstants.GAME_CANVAS_WIDTH + 40, 270), 300, 190, new Color(130, 130, 130));
            Panel graphPanel = new Panel(
                new Vector2((int)graph.Position.X, (int)graph.Position.Y), graph.Width, graph.Height, Color.DimGray);

            panels.Add(pencilPanel);
            panels.Add(graphPanel);

            coordinationLabel = new CoordinationLabel($"(x: ,y:)", new Vector2(0, 0));
        }

        public void Update(MouseState currentMouseState)
        {
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

            coordinationLabel.UpdateLabel(currentMouseState);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock, SpriteFont spriteFont)
        {
            foreach (Panel panel in panels)
            {
                panel.Draw(spriteBatch, rectangleBlock);
            }

            foreach (Button button in pencilTypeButtons)
            {
                button.Draw(spriteBatch, rectangleBlock, pencilSelected);
            }

            foreach (Button button in pencilSizeButtons)
            {
                button.Draw(spriteBatch, rectangleBlock, pencilSizeSelected);
            }

            foreach (Component component in components)
            {
                if (component is Button button)
                {
                    button.Draw(spriteBatch, rectangleBlock, spriteFont);
                }
                else if (component is Label label)
                {
                    label.Draw(spriteBatch, spriteFont);
                }
            }

            graph.Draw(spriteBatch, rectangleBlock);

            coordinationLabel.DrawCoordinate(spriteBatch, rectangleBlock, spriteFont);
        }


        public void CheckIfButtonWasClicked(MouseState currentMouseState)
        {
            foreach (Button button in GetAllButtons().Cast<Button>())
            {
                if (currentMouseState.X >= button.Position.X
                && currentMouseState.X <= button.Position.X + button.Width
                && currentMouseState.Y >= button.Position.Y
                && currentMouseState.Y <= button.Position.Y + button.Height)
                {
                    button.OnClick();
                }
            }
        }

        void DrawWithPencilAt(int x, int y)
        {
            int size = (int)pencilSizeSelected;
            for (int py = -size; py <= size; py++)
            {
                for (int px = -size; px <= size; px++)
                {
                    if (y + py >= 0 && x + px >= 0
                    && y + py < GameConstants.CELLS_VERTICALLY_COUNT
                    && x + px < GameConstants.CELLS_HORIZONTALLY_COUNT
                    && !GameManager.Instance.IsAnimalOnCell(x, y))
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
                                GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Water(GameConstants.MIN_WATER_DEPTH));
                                break;
                            case PencilType.GRASS:
                                GameManager.Instance.SetFieldCellMatter(x + px, y + py, new Grass());
                                break;
                        }
                    }
                }
            }
        }

        public void CheckIfCanDraw(MouseState currentMouseState)
        {
            if (pencilSelected != PencilType.NONE)
            {
                for (int y = 0; y < GameConstants.CELLS_VERTICALLY_COUNT; y++)
                {
                    for (int x = 0; x < GameConstants.CELLS_VERTICALLY_COUNT; x++)
                    {
                        if (currentMouseState.X >= x * GameConstants.CELL_SIZE
                        && currentMouseState.X <= x * GameConstants.CELL_SIZE + GameConstants.CELL_SIZE
                        && currentMouseState.Y >= y * GameConstants.CELL_SIZE
                        && currentMouseState.Y <= y * GameConstants.CELL_SIZE + GameConstants.CELL_SIZE)
                        {
                            DrawWithPencilAt(x, y);
                        }
                    }
                }
            }
        }
    }
}
