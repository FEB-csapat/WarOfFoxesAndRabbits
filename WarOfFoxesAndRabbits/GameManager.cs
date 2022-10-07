﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    class RabbitHandler
    {
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {

            // Rabbit dies
            if (field[x, y].inhabitant.IsDead())
            {
                field[x, y].inhabitant = null;
                return field;
            }


            if (!field[x, y].inhabitant.hasMoved)
            {
                // rules
                if (field[x, y].Grass >= 1 && field[x, y].inhabitant.Sate < 5)
                {
                    if (field[x, y].Grass == 2 && field[x, y].inhabitant.Sate < 4)
                    {
                        (field[x, y].inhabitant as Rabbit).Eat(2);
                        field[x, y].grassEaten();
                    }
                    else if (field[x, y].Grass == 2 && field[x, y].inhabitant.Sate >= 4)
                    {
                        //Can't eat sate too high!
                    }
                    else
                    {
                        (field[x, y].inhabitant as Rabbit).Eat(1);
                        field[x, y].grassEaten();
                    }
                }
                if (field[x, y].Grass < 1)
                {

                    List<Cell> surroundingCellsToMove = new List<Cell>();

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
                                if (field[x + px, y + py].inhabitant == null)
                                {
                                    surroundingCellsToMove.Add(field[x + px, y + py]);
                                }

                                // Check mates to mate with
                                if (fatherRabbit == null
                                && field[x, y].inhabitant.Sate == 5
                                && field[x + px, y + py].inhabitant != null
                                && field[x + px, y + py].inhabitant.GetType() == typeof(Rabbit)
                                && !field[x + px, y + py].inhabitant.hasProduced)
                                {
                                    if (field[x + px, y + py].inhabitant.Sate >= 4)
                                    {
                                        fatherRabbit = (Rabbit)field[x + px, y + py].inhabitant;
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
                            surroundingCellsToMove[r].inhabitant = new Rabbit();
                            surroundingCellsToMove.RemoveAt(r);

                            fatherRabbit.hasProduced = true;
                            field[x, y].inhabitant.hasProduced = true;
                        }

                        // Move rabbit to cell with best grass on it
                        if (surroundingCellsToMove.Count != 0)
                        {
                            List<Cell> optionalCells = surroundingCellsToMove.Where(x => x.Grass == surroundingCellsToMove.Max(y => y.Grass)).ToList();

                            int ran = new Random().Next(0, optionalCells.Count);

                            optionalCells[ran].inhabitant = field[x, y].inhabitant;
                            field[x, y].inhabitant = null;
                            optionalCells[ran].inhabitant.hasMoved = true;
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
                    if (field[x, y].inhabitant != null)
                    {
                        if (field[x, y].inhabitant is Rabbit)
                        {
                            field = RabbitHandler.Check(field, x, y);

                        }
                        else if (field[x, y].inhabitant is Fox)
                        {
                            // TODO: implement FoxHandler
                        }
                    }

                    field[x, y].inhabitant?.Update();
                    if (field[x, y].inhabitant == null)
                    {
                        field[x, y].Grow();
                    }
                }
            }


            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].inhabitant != null)
                    {
                        field[x, y].inhabitant.hasMoved = false;
                        field[x, y].inhabitant.hasProduced = false;
                    }
                }
            }

            return field;
        }
    }
}
