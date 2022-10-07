using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {

        public Fox()
        {
            
            sate = 4;
            age = 0;

            maxSate = 10;
            maxAge = 30;
        }

        public override Color Color => Color.Red;

        // eats rabbit
        public void Eat()
        {
            sate += 3;
        }

        public override void Eat(int food)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            sate--;
            age++;
        }
    }
}
