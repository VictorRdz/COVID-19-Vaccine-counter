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

        [HttpGet("total")]
        public ActionResult <Display> GetTotal() {
            var data = _repository.GetTotal();
            return Ok(data);
        }

        [HttpGet("people-fully")]
        public ActionResult <Display> GetPeopleFully() {
            var data = _repository.GetPeopleFully();
            return Ok(data);
        }

    }
}