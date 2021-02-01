using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MutantGeneTestClass;

namespace MutantGeneTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MutantController : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(DnaClass dna)
        {

            DnaTestClass DnaTest = new DnaTestClass(dna);

            if (DnaTest.IsMutant())
                return Ok(DnaTest.DNA);
            else
                return BadRequest(new ErrorEvelope { Error = DnaTest.Error });

        }

    }
}
