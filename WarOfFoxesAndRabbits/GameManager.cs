using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    class RabbitHandler
    {
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {
            Rabbit rabbitOnCurrentCell = (Rabbit)field[x, y].animal;

            // Rabbit dies
            if (rabbitOnCurrentCell.IsDead())
            {
                field[x, y].animal = null;
                return field;
            }


            if (!rabbitOnCurrentCell.hasMoved)
            {
                // rules
                if (field[x, y].matter is Grass)
                {
                    Grass grass = (Grass)field[x, y].matter;

                    if (grass.Stage >= 1 && rabbitOnCurrentCell.Sate < 5)
                    {
                        if (grass.Stage == 2 && rabbitOnCurrentCell.Sate < 4)
                        {
                            (field[x, y].animal as Rabbit).Eat(2);
                            grass.grassEaten();
                        }
                        else if (grass.Stage == 2 && rabbitOnCurrentCell.Sate >= 4)
                        {
                            //Can't eat sate too high!
                        }
                        else
                        {
                            (field[x, y].animal as Rabbit).Eat(1);
                            grass.grassEaten();
                        }
                    }
                    if (grass.Stage < 1)
                    {
                        List<Cell> surroundingCellsToMove = new List<Cell>();

                        List<Cell> surroundingCellsToMoveWater = new List<Cell>();

                        Rabbit fatherRabbit = null;

                        for (int py = -1; py <= 1; py++)
                        {
                            for (int px = -1; px <= 1; px++)
                            {
                                if (y + py >= 0 && x + px >= 0
                                && y + py < GameVariables.CellsVerticallyCount && x + px < GameVariables.CellsHorizontallyCount
                                && (px != 0 || py != 0))
                                {
                                    // Check cells to move
                                    if (field[x + px, y + py].animal == null)
                                    {
                                        if (field[x + px, y + py].matter is Water)
                                        {
                                            surroundingCellsToMoveWater.Add(field[x + px, y + py]);
                                        }
                                        else if (field[x + px, y + py].matter is Grass)
                                        {
                                            surroundingCellsToMove.Add(field[x + px, y + py]);
                                        }
                                    }

                                    // Check mates to mate with
                                    if (fatherRabbit == null
                                    && rabbitOnCurrentCell.Sate == 5
                                    && field[x + px, y + py].animal != null
                                    && field[x + px, y + py].animal.GetType() == typeof(Rabbit)
                                    && !((Rabbit)field[x + px, y + py].animal).hasProduced)
                                    {
                                        if (((Rabbit)field[x + px, y + py].animal).Sate >= 4)
                                        {
                                            fatherRabbit = (Rabbit)field[x + px, y + py].animal;
                                        }
                                    }
                                }
                            }
                        }

                        if (surroundingCellsToMove.Count != 0)
                        {
                            // Birth rabbit to empty cell
                            if (fatherRabbit != null)
                            {
                                int r = new Random().Next(0, surroundingCellsToMove.Count);
                                surroundingCellsToMove[r].animal = new Rabbit();
                                surroundingCellsToMove.RemoveAt(r);

                                fatherRabbit.hasProduced = true;
                                rabbitOnCurrentCell.hasProduced = true;
                            }

                            // Move rabbit to cell with best grass on it
                            if (surroundingCellsToMove.Count != 0)
                            {
                                List<Cell> optionalCells = new List<Cell>();
                                bool hasWaterCell = false;

                                // TODO: sometimes throws exception
                                double bestGrassStage = Math.Floor(surroundingCellsToMove.Where(x => x.matter is Grass).Max(y => ((Grass)y.matter).Stage));
                                foreach (Cell cell in surroundingCellsToMove)
                                {
                                    if (cell.matter is Water && !hasWaterCell)
                                    {
                                        hasWaterCell = false;
                                        optionalCells.Add(cell); ;
                                    }

                                    if (cell.matter is Grass && ((Grass)cell.matter).Stage >= bestGrassStage)
                                    {
                                        optionalCells.Add(cell); ;
                                    }
                                }

                                optionalCells.AddRange(surroundingCellsToMoveWater);

                                int ran = new Random().Next(0, optionalCells.Count);

                                optionalCells[ran].animal = field[x, y].animal;
                                field[x, y].animal = null;
                                ((Rabbit)optionalCells[ran].animal).hasMoved = true;
                            }

                        }
                    }
                }

            }
            return field;
        }
    }
    class FoxHandler
    {

    }

    // Manages the game's rules
    class GameManager
    {
        // Modify the fields according to the rules
        public static Cell[,] Update(Cell[,] field)
        {
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null)
                    {
                        if (field[x, y].animal is Rabbit)
                        {
                            field = RabbitHandler.Check(field, x, y);

                        }
                        else if (field[x, y].animal is Fox)
                        {
                            // TODO: implement FoxHandler
                        }
                    }

                    if (field[x, y].animal != null)
                    {
                        field[x, y].animal.Update();
                    }
                    else if (field[x, y].matter is Grass)
                    {
                        ((Grass)field[x, y].matter).Grow();
                    }

                }
            }


            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null && field[x, y].animal is Animal)
                    {
                        ((Animal)field[x, y].animal).hasMoved = false;
                        ((Animal)field[x, y].animal).hasProduced = false;
                    }
                }
            }

            return field;
        }
    }
}
