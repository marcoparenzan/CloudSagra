using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CloudSagra.OrderApp.Models
{
    public class RigaOrdine
    {
        [Key]
        public int Id { get; set; }
        public int NumeroTavolo { get; set; }
        public DateTime DataOrdine { get; set; }
        public string Codice { get; set; }
        public string Categoria { get; set; }
        public int Quantita { get; set; }
        public decimal Prezzo { get; set; }
    }
}
