using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MutantGeneTestAPI
{

    public class MutantDNADBContext : DbContext
    {
        public MutantDNADBContext(DbContextOptions<MutantDNADBContext> options)
            : base(options) { }

        public DbSet<DnaEntityClass> DNA { get; set; }

    }

    public class DnaEntityClass
    {

        [Key]
        public string Signature { get; set; }

        [Column]
        public bool IsMutant { get; set; }

    }

}
