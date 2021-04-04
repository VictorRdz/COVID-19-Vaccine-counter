namespace covid19_backend.Data
{
    static public class TestNumber {
        private static ulong deaths = 1000000;

        public static void UpdateDeaths() {
            deaths += 0;
        }

        public static ulong GetDeaths() {
            return deaths;
        }

    }
}