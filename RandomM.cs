using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomNumberApp
{
    class RandomM
    {
        private decimal[] _randomDecimals = new decimal[30];
        private readonly Dictionary<int, int> _probability;

        private decimal _space;

        public RandomM(Dictionary<int, int> probability)
        {
            generateDoubles();
            _probability = probability;
        }

        public HashSet<int> Randomize()
        {
            CalculateSpace();
            HashSet<int> people = new HashSet<int>();
            foreach (var random in _randomDecimals)
            {
                people.Add(GetPerson(random));
            }
            return people;
        }

        private void generateDoubles()
        {
            var rn = new Random();
            for (int i = 0; i < 30; i++)
            {
                _randomDecimals[i] = Convert.ToDecimal(rn.NextDouble());
            }
        }

        private void CalculateSpace()
        {
            int countOfReducedProbability = _probability.Count(k => k.Value == 1);

            int segments = 2 * _probability.Count - countOfReducedProbability;

            _space = (decimal) 1 / segments;
        }

        private int GetPerson(decimal generatedNumber)
        {
            decimal lastPos = 0;
            decimal currentPos = 0;
            for (int i = 1; i <= _probability.Count; i++)
            {
                currentPos +=_space * _probability[i];
                if (generatedNumber >= lastPos && generatedNumber < currentPos)
                    return i;
                lastPos = currentPos;
            }
          
            return -1; 
        }
    }
}
