using System.Collections.Generic;
using covid19_backend.Models;
using covid19_backend.Tasks;

namespace covid19_backend.Data
{
    public class MockCovidRepo : ICovidRepo
    {
        public IEnumerable<Covid> GetAll()
        {
            return new List<Covid>
            {
                new Covid { Confirmed=0, Deaths=0, Recovered=0, Vaccined=0, Error=0 },
                new Covid { Confirmed=0, Deaths=0, Recovered=0, Vaccined=0, Error=0 },
                new Covid { Confirmed=0, Deaths=0, Recovered=0, Vaccined=0, Error=0 }
            };
        }

        public Covid GetZone(string zone)
        {
            return new Covid { Confirmed=0, Deaths=0, Recovered=0, Vaccined=UpdateData.GetDisplay(), Error=0 };
        }
    }
}