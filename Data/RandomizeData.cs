using System;

namespace covid19_backend.Data
{
    public class RandomizeData {
        
        public static int seed = 55555;
        public static Random rand = new Random(RandomizeData.seed);
        
        public static double GetInitialValue(double initialValue, double finalValue, int timeInSeconds) {
            double result = initialValue;
            double total = finalValue - initialValue;
            double day = 86400;
            for(int i = 0; i <= timeInSeconds; i++) {
                double cantidad = rand.NextDouble() * total * (2 / day);
                result += cantidad;
            }
            return result;
        }

        public static double GetUpdateRandom(double initialValue, double finalValue) {
            double total = finalValue - initialValue;
            double day = 86400;
            return rand.NextDouble() * total * (2 / day);
        }


    }
}