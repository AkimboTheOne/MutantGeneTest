using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutantGeneTestAPI
{
    public class DnaStatsClass
    {

        public int count_mutant_dna { get; set; }
        public int count_human_dna { get; set; }
        public double ratio { get { return count_human_dna != 0 ? Convert.ToDouble(count_human_dna) / Convert.ToDouble(count_mutant_dna) : 0; } }

    }
}
