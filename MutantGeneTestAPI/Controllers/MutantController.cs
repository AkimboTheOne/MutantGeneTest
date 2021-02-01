using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MutantGeneTestClass;

namespace MutantGeneTestAPI.Controllers
{
//    [Route("api/[controller]")]
    [ApiController]
    public class MutantController : ControllerBase
    {


        /// <summary>
        /// máximo número de cadenas que se pueden persistir en el contexto
        /// </summary>
        static int _maxMutantDNADBContextCapacity = 10000;


        // dna db contexto
        private MutantDNADBContext _context;
        public MutantController(MutantDNADBContext context)
        {
            _context = context;
        }


        /// <summary>
        /// función de persistencia de cadenas DNA
        /// </summary>
        /// <param name="dnatest"></param>
        void DnaPersit(ref DnaTestClass dnatest)
        {
            try
            {
                // GUARDIANES
                if (_context.DNA.Count() >= _maxMutantDNADBContextCapacity) throw new ApplicationException("NO SE PUEDEN ALMACENAR MÁS DNAS PARA ESTA SESIÓN");

                _context.DNA.Add(new DnaEntityClass { Signature = dnatest.DNASignature, IsMutant = dnatest.IsMutant() });
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        DnaStatsClass DnaStats()
        {
            return new DnaStatsClass()
            {
                count_human_dna = _context.DNA.Where(i => i.IsMutant == false).Count(),
                count_mutant_dna = _context.DNA.Where(i => i.IsMutant == true).Count()
            };
        }


        [HttpPost]
        [Route("api/mutant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(DnaClass dna)
        {
            try
            {
                DnaTestClass DnaTest = new DnaTestClass(dna);

                if (DnaTest.IsMutant())
                {
                    DnaPersit(ref DnaTest);
                    return Ok(DnaTest.DNA);
                }
                else
                {
                    DnaPersit(ref DnaTest);
                    return BadRequest(new ErrorEnvelopeClass { Error = DnaTest.Error });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorEnvelopeClass { Error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/mutant/stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetStats()
        {
            try
            {
                return Ok(DnaStats());
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorEnvelopeClass { Error = ex.Message });
            }
        }


    }
}
