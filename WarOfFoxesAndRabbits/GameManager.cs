using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    // Manages the game's rules
    public sealed class GameManager
    {

        #region Counters
        public int RabbitCounter { get; private set; } = 0;
        public int FoxCounter { get; private set; } = 0;


        public long FoxDeathCounter { get; private set; } = 0;
        public long RabbitDeathCounter { get; private set; } = 0;

        public void IncrementFoxDeathCounter()
        {
            FoxDeathCounter++;
        }

        public void IncrementRabbitDeathCounter()
        {
            RabbitDeathCounter++;
        }

        public void ResetDeathCounters()
        {
            FoxDeathCounter = 0;
            RabbitDeathCounter = 0;
        }
        #endregion

        #region Ticks

        public long TickCounter { get; set; } = 0;
        public long Tickrate { get; private set; } = 1;

        public void SetTickrateToMinimum()
        {
            Tickrate = 1;
        }

        public void SetTickrateToMaximum()
        {
            Tickrate = 60;
        }

        public void IncrementTickrate()
        {
            if (Tickrate + 2 <= 60)
            {
                Tickrate += 2;
            }
            else
            {
                Tickrate = 60;
            }
        }

        public void DecrementTickrate()
        {
            if (Tickrate - 2 > 0)
            {
                Tickrate -= 2;
            }
            else
            {
                Tickrate = 1;
            }
        }

        #endregion

        #region Generation

        public int Generation { get; private set; } = 0;

        public void IncrementGeneration()
        {
            Generation++;
        }

        public void ResetGeneration()
        {
            Generation = 0;
        }

        #endregion

        #region Field

        private readonly Cell[,] field = new Cell[GameConstants.CELLS_HORIZONTALLY_COUNT, GameConstants.CELLS_VERTICALLY_COUNT];

        public void SetFieldCellAnimal(int x, int y, Animal animal)
        {
            field[x, y].Animal = animal;
        }

        public void SetFieldCellMatter(int x, int y, Matter matter)
        {
            field[x, y].Matter = matter;
        }

        public bool IsAnimalOnCell(int x, int y)
        {
            return field[x, y].Animal is not null;
        }

        public void ClearAnimals()
        {
            for (int y = 0; y < GameConstants.CELLS_VERTICALLY_COUNT; y++)
            {
                for (int x = 0; x < GameConstants.CELLS_VERTICALLY_COUNT; x++)
                {
                    if (field[x, y].Animal is not null)
                    {
                        field[x, y].Animal = null;
                    }
                }
            }
        }

        public void GenerateField()
        {
            FastNoiseLite noise = new FastNoiseLite();
            if (IsLakeEnabled)
            {
                noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
                noise.SetSeed(GameConstants.Random.Next(0, 1000000));
            }

            for (int y = 0; y < GameConstants.CELLS_VERTICALLY_COUNT; y++)
            {
                for (int x = 0; x < GameConstants.CELLS_VERTICALLY_COUNT; x++)
                {
                    bool hasWater = false;

                    float waterDepth = noise.GetNoise(x, y);

                    if (IsLakeEnabled && waterDepth >= GameConstants.MINT_WATER_DEPTH)
                    {
                        hasWater = true;
                    }

                    field[x, y] = new Cell(new Vector2(x, y), matter: hasWater ? new Water(waterDepth) : null);
                }
            }
        }

        #endregion

        public bool IsPaused { get; set; } = false;
        public bool IsLakeEnabled { get; set; } = false;


        private readonly FoxHandler foxHandler = new FoxHandler();
        private readonly RabbitHandler rabbitHandler = new RabbitHandler();

        #region Singleton
        private static GameManager instance = null;

        private GameManager() { }

        public static GameManager Instance
        {
            get
            {
                instance ??= new GameManager();
                return instance;
            }
        }
        #endregion

        public void Initialize()
        {
            GenerateField();
        }

        // Update the fields according to the rules
        public void Update()
        {
            RabbitCounter = 0;
            FoxCounter = 0;
            for (int y = 0; y < GameConstants.CELLS_VERTICALLY_COUNT; y++)
            {
                for (int x = 0; x < GameConstants.CELLS_HORIZONTALLY_COUNT; x++)
                {
                    if (field[x, y].Animal != null)
                    {
                        field[x, y].Animal.Update();
                        if (field[x, y].Animal is Rabbit)
                        {
                            rabbitHandler.Manage(field, x, y);
                            RabbitCounter++;
                        }
                        else if (field[x, y].Animal is Fox)
                        {
                            foxHandler.Manage(field, x, y);
                            FoxCounter++;
                        }
                    }
                    else if (field[x, y].Matter is Grass grass)
                    {
                        grass.Grow();
                    }
                }
            }

            // Reset properties of animals
            for (int y = 0; y < GameConstants.CELLS_VERTICALLY_COUNT; y++)
            {
                for (int x = 0; x < GameConstants.CELLS_HORIZONTALLY_COUNT; x++)
                {
                    if (field[x, y].Animal != null)
                    {
                        field[x, y].Animal.HasMoved = false;
                        field[x, y].Animal.HasProduced = false;
                    }
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock)
        {
            for (int y = 0; y < GameConstants.CELLS_VERTICALLY_COUNT; y++)
            {
                for (int x = 0; x < GameConstants.CELLS_VERTICALLY_COUNT; x++)
                {
                    spriteBatch.Draw(rectangleBlock,
                        new Rectangle(x * GameConstants.CELL_SIZE, y * GameConstants.CELL_SIZE,
                                      GameConstants.CELL_SIZE, GameConstants.CELL_SIZE), field[x, y].Color);
                }
            }
        }

    }
}
