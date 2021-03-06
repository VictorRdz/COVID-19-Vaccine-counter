using System;

namespace covid19_backend.Services
{
    public class Randomize
    {
        public static int seed = 2837;
        public static double GetInitial(double startValue, double finalValue, int time, Random rand) {
            double result = 0;
            for(int i = 0; i < time; i++) {
                result += GetIncrement(startValue, finalValue, rand);
            }
            return Math.Round(startValue + result);
        }

        public static Random GetRandom() {
            return new Random(seed);
        }

        public static double GetIncrement(double startValue, double finalValue, Random rand) {
            double value = 0;
            double increment = (finalValue - startValue) / 86400;
            int randomValue = rand.Next(7);
            switch (randomValue)
            {
                case 0:
                    value = increment * 0.25; 
                    break;
                case 1:
                    value = increment * 0.50; 
                    break;
                case 2:
                    value = increment * 0.75; 
                    break;
                case 3:
                    value = increment * 1.00; 
                    break;
                case 4:
                    value = increment * 1.25; 
                    break;
                case 5:
                    value = increment * 1.50; 
                    break;
                case 6:
                    value = increment * 1.75; 
                    break;
                default:
                    break;
            }
            return value;
        }
    }
}