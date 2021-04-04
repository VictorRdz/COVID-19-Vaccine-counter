using System.Collections.Generic;
using covid19_backend.Models;

namespace covid19_backend.Data
{
    public interface ICovidRepo {
        IEnumerable<Covid> GetAll();
        Covid GetZone(string zone);
    }
}