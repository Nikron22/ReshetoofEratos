namespace Prog.Services
{
    public class SieveOfEratosthenes
    {
        private readonly int _limit;
        private readonly List<int> _primes;

        public SieveOfEratosthenes(int limit)
        {
            _limit = limit;
            _primes = new List<int>();
            FindPrimes();
        }

        private void FindPrimes()
        {
            if (_limit < 2)
                return;

            bool[] isPrime = new bool[_limit + 1];
            for (int i = 2; i <= _limit; i++)
            {
                isPrime[i] = true;
            }

            for (int p = 2; p * p <= _limit; p++)
            {
                if (isPrime[p])
                {
                    for (int i = p * p; i <= _limit; i += p)
                    {
                        isPrime[i] = false;
                    }
                }
            }

            for (int i = 2; i <= _limit; i++)
            {
                if (isPrime[i])
                {
                    _primes.Add(i);
                }
            }
        }

        public List<int> GetPrimes()
        {
            return _primes;
        }

        public string GetPrimesAsString()
        {
            return string.Join(", ", _primes);
        }
    }
}