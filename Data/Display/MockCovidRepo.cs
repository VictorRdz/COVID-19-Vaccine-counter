using System.Collections.Generic;
using covid19_backend.Models;
using covid19_backend.Tasks;

namespace covid19_backend.Data
{
    public class MockCovidRepo : ICovidRepo
    {
        public Display GetTotal()
        {
            return new Display { Value=UpdateData.counterTotal.GetDisplay(), PredictionError=UpdateData.counterTotal.PredictionError, DataError=UpdateData.counterTotal.DataError };
        }

        public Display GetPeopleFully()
        {
            return new Display { Value=UpdateData.counterPeopleFully.GetDisplay(), PredictionError=UpdateData.counterPeopleFully.PredictionError, DataError=UpdateData.counterPeopleFully.DataError };
        }
    }
}