using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfFoxesAndRabbits
{
    // Manage rabbits according to the rules
    class RabbitHandler : AnimalHandler<Rabbit>
    {
        bool CanEat(Cell[,] field, int x, int y)
        {
            return field[x, y].Matter is Grass grass
                && grass.Stage >= 1
                && field[x, y].Animal.CanEat();
        }

        void Eat(Rabbit rabbit, Grass grass)
        {
            if (grass.Stage >= 2)
            {
                rabbit.Eat(2);
                grass.GrassEaten(2);
            }
            else
            {
                rabbit.Eat(1);
                grass.GrassEaten(1);
            }
        }

        List<Cell> GetEmptyCellsToMove(List<Cell> emptySurroundingCells)
        {
            // Move rabbit to cell with best grass on it
            List<Cell> optionalCells = new List<Cell>();

            if (emptySurroundingCells.Exists(x => x.Matter != null && x.Matter is Grass))
            {
                double bestGrassStage = Math.Floor(emptySurroundingCells.Where(x => x.Matter is Grass).Max(y => ((Grass)y.Matter).Stage));
                foreach (Cell cell in emptySurroundingCells)
                {
                    if (cell.Matter is not Wall)
                    {
                        if (cell.Matter is Grass && ((Grass)cell.Matter).Stage >= bestGrassStage)
                        {
                            optionalCells.Add(cell);
                        }
                    }
                }
            }
            else
            {
                return emptySurroundingCells;
            }
            return optionalCells;
        }

        private void FindSurroundingCells(Cell[,] field, int x, int y, List<Cell> emptySurroundingCells, ref Rabbit fatherRabbit)
        {
            for (int py = -1; py <= 1; py++)
            {
                for (int px = -1; px <= 1; px++)
                {
                    if (y + py >= 0 && x + px >= 0
                    && y + py < GameConstants.CELLS_VERTICALLY_COUNT
                    && x + px < GameConstants.CELLS_HORIZONTALLY_COUNT
                    && (px != 0 || py != 0))
                    {
                        #region Find cells to move

                        if (field[x + px, y + py].Animal == null
                        && field[x + px, y + py].Matter is not Wall)
                        {
                            emptySurroundingCells.Add(field[x + px, y + py]);
                        }

                        #endregion

                        #region Find father

                        if (fatherRabbit == null
                        && field[x + px, y + py].Animal is Rabbit
                        && CanBeFather((Rabbit)field[x, y].Animal, (Rabbit)field[x + px, y + py].Animal))
                        {
                            fatherRabbit = (Rabbit)field[x + px, y + py].Animal;
                        }

                        #endregion
                    }
                }
            }
        }

        // Perform rules on each cell containing a rabbit
        public override Cell[,] Manage(Cell[,] field, int x, int y)
        {
            if (field[x, y].Animal.HasMoved)
            {
                return field;
            }

            if (field[x, y].Animal.IsDead())
            {
                field[x, y].Animal = null;
                GameManager.Instance.IncrementRabbitDeathCounter();
                return field;
            }

            if (CanEat(field, x, y))
            {
                Eat((Rabbit)field[x, y].Animal, (Grass)field[x, y].Matter);
            }

            List<Cell> emptySurroundingCells = new List<Cell>();

            Rabbit fatherRabbit = null;
            FindSurroundingCells(field, x, y, emptySurroundingCells, ref fatherRabbit);

            if (emptySurroundingCells.Count > 0)
            {
                Rabbit motherRabbit = (Rabbit)field[x, y].Animal;
                if (CanBirth(emptySurroundingCells, motherRabbit, fatherRabbit))
                {
                    Birth(emptySurroundingCells, motherRabbit, fatherRabbit);
                }
            }

            if (emptySurroundingCells.Count > 0)
            {
                Move(GetEmptyCellsToMove(emptySurroundingCells), field[x, y]);
            }

            return field;
        }
    }
}
