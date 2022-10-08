namespace WarOfFoxesAndRabbits
{
    public abstract class Animal : Entity
    {
        protected int sate;
        protected int maxSate;
        protected int age;
        protected int maxAge;


        public bool hasMoved = false;
        public bool hasProduced = false;

        public int Sate => sate;

        public int Age => age;

        public abstract void Eat(int amount);

        public virtual bool IsDead() => Sate <= 0 || age >= maxAge;


        // Minden ami egy generáció alatt történik, az az updateban kell legyen. pl: evés, szaporodás
        public virtual void Update()
        {
            sate--;
            age++;
        }
    }
}
