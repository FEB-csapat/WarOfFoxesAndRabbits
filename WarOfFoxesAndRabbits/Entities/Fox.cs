using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {

        public Fox()
        {
            sate = 18;
            age = 0;

            maxSate = 30;
            maxAge = 120;

            hasAte = true;
            hasMoved = true;
            hasProduced = true;
        }

        public override Color Color => Color.Red;

        public override bool canBreed()
        {
            return sate >= 16;
        }

        // eats rabbit
        public void Eat()
        {
            if (sate < maxSate){
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
