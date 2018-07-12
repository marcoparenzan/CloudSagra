using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudSagra.OrderApp.Models
{
    public class NuovoOrdine
    {
        public int NumeroTavolo { get; set; }

        public NuovaRigaOrdine[] Righe { get; set; }
    }

    public class NuovaRigaOrdine
    {
        public string Codice { get; set; }
        public int? Quantita { get; set; }
        public string Categoria { get; set; }
        public decimal Prezzo { get; set; }
    }
}
