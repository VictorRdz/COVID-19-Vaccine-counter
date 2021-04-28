using System.Threading;

namespace covid19_backend.Models
{
    public class Counter
    {
        public Counter(string type) {
            this.Type = type;
        }
        public double Display { get; set; } = 0;
        public double Prediction { get; set; } = 0;
        public double PreviousPrediction { get; set; } = 0;
        public double Previous { get; set; } = 0;
        public string Error { get; set; }
        public string DataError { get; set; }
        public string Type { get; set; }
        public Timer Updater { get; set; }
        public double GetDisplay() {
            return System.Math.Round(this.Display);
        }

    }
}