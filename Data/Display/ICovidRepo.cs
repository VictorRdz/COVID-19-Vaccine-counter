using System.Collections.Generic;
using covid19_backend.Models;

namespace covid19_backend.Data
{
    public interface ICovidRepo {
        Display GetTotal();
        Display GetPeopleFully();
    }
}