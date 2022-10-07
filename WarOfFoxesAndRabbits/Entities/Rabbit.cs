using Microsoft.Xna.Framework;
namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        public Rabbit()
        {
            sate = 3;
            age = 0;
            maxSate = 5;
            maxAge = 30;
        }

        public override Color Color => Color.White;

        public override void Eat(int amount)
        {
            if (sate < maxSate)
            {
                sate += amount;
            }
        }

        public override void Update()
        {
            sate--;
            age++;
        }
    }
}
