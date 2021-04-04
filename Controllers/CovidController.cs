using System.Collections.Generic;
using covid19_backend.Data;
using covid19_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace covid19_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidController : ControllerBase {
        private readonly ICovidRepo _repository;

        public CovidController(ICovidRepo repository){
            _repository = repository;
        }

        [HttpGet]
        public ActionResult <IEnumerable<Covid>> GetAll() {
            var allData = _repository.GetAll();
            return Ok(allData);
        }

        [HttpGet("{zone}")]
        public ActionResult <Covid> GetZone(string zone) {
            var data = _repository.GetZone(zone);
            return Ok(data);
        }

    }
}