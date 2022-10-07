using Microsoft.Xna.Framework;
namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        public Rabbit()
        {
           // System.Diagnostics.Debug.WriteLine("rabbit created");
            sate = 3;
            age = 0;
            maxSate = 5;
            maxAge = 35;
        }

        Color color =  new Color(255, 255, 255);
        public Rabbit(Color color)
        {
          //  System.Diagnostics.Debug.WriteLine("rabbit created");
            sate = 1;
            age = 0;
            maxSate = 5;
            maxAge = 35;

            this.color = color;
            
        }

        public override Color Color => color;

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
