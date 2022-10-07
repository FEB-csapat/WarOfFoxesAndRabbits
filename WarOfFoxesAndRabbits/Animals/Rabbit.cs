using Microsoft.Xna.Framework;
namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        public Rabbit()
        {
            Color = new Color(255, 255, 255);
            sate = 1;
            age = 0;

            maxSate = 5;
            maxAge = 25;
        }

        public void Eat()
        {
            sate++;
        }

        public override void Update()
        {
            sate--;
            age++;
        }
    }
}
