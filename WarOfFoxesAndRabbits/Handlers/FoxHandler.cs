using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    // Manage foxes according to the rules
    class FoxHandler : AnimalHandler<Fox>
    {
        // Fills the surroundingCells lists to perform actions latter on it
        private void FindSurroundingCells(Cell[,] field, int x, int y,
            out List<Cell> surroundingCellsToMove, out List<Cell> surroundingCellsToHunt, ref Fox fatherFox)
        {
            Cell currentCell = field[x, y];

            surroundingCellsToMove = new List<Cell>();
            surroundingCellsToHunt = new List<Cell>();

            int vision = 2;

            if (currentCell.Animal.Sate < 20)
            {
                vision = 3;
            }
            else if (currentCell.Animal.Sate < 10)
            {
                vision = 4;
            }

            for (int py = -vision; py <= vision; py++)
            {
                for (int px = -vision; px <= vision; px++)
                {
                    if (y + py >= 0 && x + px >= 0
                    && y + py < GameConstants.CELLS_VERTICALLY_COUNT
                    && x + px < GameConstants.CELLS_HORIZONTALLY_COUNT
                    && (px != 0 || py != 0))
                    {
                        Cell currentSurroundingCell = field[x + px, y + py];
                        // Find cells to move
                        if (currentSurroundingCell.Animal == null)
                        {
                            if (currentSurroundingCell.Matter is not Wall)
                            {
                                surroundingCellsToMove.Add(currentSurroundingCell);
                            }
                        }
                        // Find cells to hunt
                        else if (currentSurroundingCell.Animal is Rabbit
                             && currentCell.Animal.CanEat())
                        {
                            surroundingCellsToHunt.Add(field[x + px, y + py]);
                        }

                        // Find mates to mate with
                        if (fatherFox == null && currentSurroundingCell.Animal is Fox
                        && CanBeFather((Fox)currentCell.Animal, (Fox)currentSurroundingCell.Animal))
                        {
                            fatherFox = (Fox)currentSurroundingCell.Animal;
                        }
                    }
                }
            }
        }

        private Cell Hunt(List<Cell> surroundingCellsToHunt, Cell cellWithFox)
        {
            int ran = GameConstants.Random.Next(0, surroundingCellsToHunt.Count);
            (cellWithFox.Animal as Fox).Eat();
            cellWithFox.Animal.HasAte = true;
            surroundingCellsToHunt[ran].Animal = cellWithFox.Animal;
            cellWithFox.Animal = null;

            // Return where it moved
            return surroundingCellsToHunt[ran];
        }

        // Perform rules on each cell containing a fox
        public override Cell[,] Manage(Cell[,] field, int x, int y)
        {
            if (field[x, y].Animal.HasMoved)
            {
                return field;
            }

            if (field[x, y].Animal.IsDead())
            {
                field[x, y].Animal = null;
                GameManager.Instance.IncrementFoxDeathCounter();
                return field;
            }

            Cell nextCellWhereFoxMoved;
            List<Cell> surroundingCellsToMove, surroundingCellsToHunt;
            Fox fatherFox = null;
            FindSurroundingCells(field, x, y, out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);

            if (surroundingCellsToMove.Count > 0)
            {
                Fox motherFox = (Fox)field[x, y].Animal;
                if (CanBirth(surroundingCellsToMove, motherFox, fatherFox))
                {
                    Birth(surroundingCellsToMove, motherFox, fatherFox);
                }
            }

            // Check if there are any rabbits to hunt in the surrounding cells
            if (surroundingCellsToHunt.Count > 0)
            {
                nextCellWhereFoxMoved = Hunt(surroundingCellsToHunt, field[x, y]);

                // has moved successfully

                FindSurroundingCells(field, nextCellWhereFoxMoved.RowPosition, nextCellWhereFoxMoved.ColumnPosition,
                    out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);
                if (surroundingCellsToMove.Count > 0)
                {
                    nextCellWhereFoxMoved = Move(surroundingCellsToMove,
                        field[nextCellWhereFoxMoved.RowPosition, nextCellWhereFoxMoved.ColumnPosition]);
                }

            }
            // There were no rabbits in the surrounding cells, so just move if there is a cell to move
            else if (surroundingCellsToMove.Count > 0)
            {
                nextCellWhereFoxMoved = Move(surroundingCellsToMove, field[x, y]);

                FindSurroundingCells(field, nextCellWhereFoxMoved.RowPosition, nextCellWhereFoxMoved.ColumnPosition,
                    out surroundingCellsToMove, out surroundingCellsToHunt, ref fatherFox);

                //Check if it can hunt or not
                if (surroundingCellsToHunt.Count > 0)
                {
                    nextCellWhereFoxMoved = Hunt(surroundingCellsToHunt,
                        field[nextCellWhereFoxMoved.RowPosition, nextCellWhereFoxMoved.ColumnPosition]);
                }
                else
                {
                    //It can't hunt, so it makes the second move
                    if (surroundingCellsToMove.Count > 0)
                    {
                        nextCellWhereFoxMoved = Move(surroundingCellsToMove,
                            field[nextCellWhereFoxMoved.RowPosition, nextCellWhereFoxMoved.ColumnPosition]);
                    }
                }
            }
            return field;
        }
    }
}
