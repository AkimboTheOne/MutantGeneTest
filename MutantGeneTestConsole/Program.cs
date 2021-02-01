using System;

using MutantGeneTestClass;

namespace MutantGeneTestConsole
{
    class Program
    {

        static void Main(string[] args)
        {

            DnaTestClass _test = new DnaTestClass(new string[] { "ATGCGA", "CAGTGC", "TTATGT", "AGAAGG", "CCCCTA", "TCACTG" });
            //DnaTestClass _test = new DnaTestClass(new string[] { "ATGCG", "CAGTG", "TTATG", "AGAAG", "CCCCT" });

            Console.WriteLine("-- ¿¿ es una cadena dna ?? --");
            Console.WriteLine(_test.IsValid);

            Console.WriteLine("-- cadena dna --");
            foreach (var item in _test.DNA)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine("-- ¿¿ es mutante ?? --");

            Console.WriteLine(_test.IsMutant());

        }
    }
}
