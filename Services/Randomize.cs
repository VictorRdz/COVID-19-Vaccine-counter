using System;

namespace covid19_backend.Services
{
    public class Randomize
    {
        public static int seed = 2837;
        public static Random rand = new Random();
        public static double GetInitial(double startValue, double finalValue, int time) {
            double result = 0;

            ResetRandom();
            for(int i = 0; i < time; i++) {
                result += GetIncrement(startValue, finalValue);
            }
            return Math.Round(startValue + result);
        }

        public static void ResetRandom() {
            rand = new Random(seed);
            return;
        }

        public static double GetIncrement(double startValue, double finalValue) {
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