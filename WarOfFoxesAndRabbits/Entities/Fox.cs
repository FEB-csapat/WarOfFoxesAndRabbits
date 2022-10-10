using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {

        public Fox()
        {
            sate = 7;
            age = 0;

            maxSate = 10;
            maxAge = 45;
        }


        public override Color Color => Color.Red;

        // eats rabbit
        public void Eat()
        {
            if (sate < maxSate)
            {
                sate += 3;
            }
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
