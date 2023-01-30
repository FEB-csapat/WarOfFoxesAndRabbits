using System;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    abstract class AnimalHandler<T> where T : Animal
    {
        public abstract Cell[,] Manage(Cell[,] field, int x, int y);

        protected void Birth(List<Cell> surroundingCellsToBirth,
            T mother, T father)
        {
            int r = GameConstants.Random.Next(0, surroundingCellsToBirth.Count);
            surroundingCellsToBirth[r].Animal = Activator.CreateInstance<T>();
            surroundingCellsToBirth.RemoveAt(r);
            father.HasProduced = true;
            mother.HasProduced = true;
        }

        protected bool CanBirth(List<Cell> surroundingCellsToBirth,
            T mother, T father)
        {
            return father != null && mother != null && surroundingCellsToBirth.Count > 0;
        }

        protected Cell Move(List<Cell> surroundingCellsToMove, Cell cellWithAnimal)
        {
            int ran = GameConstants.Random.Next(0, surroundingCellsToMove.Count);
            surroundingCellsToMove[ran].Animal = cellWithAnimal.Animal;
            surroundingCellsToMove[ran].Animal.HasMoved = true;
            cellWithAnimal.Animal = null;

            // Return where it moved
            return surroundingCellsToMove[ran];
        }

        protected bool CanBeFather(T mother, T possibleFather)
        {
            if (possibleFather != null
                && mother.CanBreed()
                && possibleFather.CanBreed()
                && possibleFather is T
                && !possibleFather.HasProduced)
            {
                return true;
            }
            return false;
        }
    }
}
