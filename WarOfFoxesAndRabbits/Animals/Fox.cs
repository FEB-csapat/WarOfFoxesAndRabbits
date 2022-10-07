using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {

        public Fox()
        {
            Color = Color.Red;
            sate = 4;
            age = 0;

            maxSate = 10;
            maxAge = 30;
        }

        // eats rabbit
        public void Eat()
        {
            sate += 3;
        }

        public override void Update()
        {
            sate--;
            age++;
        }
    }
}
