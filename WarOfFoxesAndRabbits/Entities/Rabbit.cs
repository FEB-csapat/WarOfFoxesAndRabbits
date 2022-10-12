using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        private Color color;
        public Rabbit()
        {
            sate = 3;
            age = 0;
            maxSate = 9;
            maxAge = 60;

            hasAte = true;
            hasMoved = true;
            hasProduced = true;

            //  int c = GameVariables.Random.Next(170, 255);
            //  color = new Color(c, c, c);
        }

        public override Color Color => Color.White;

        public override bool canBreed()
        {
            return sate >= 4;
        }

        public override void Eat(int amount)
        {
            if (sate < maxSate)
            {
                sate += amount;
            }
        }
    }
}
