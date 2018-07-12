using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudSagra.OrderApp.Models
{
    public class NuovaComanda
    {
        public int NumeroTavolo { get; set; }

        public NuovaRigaComanda[] Righe { get; set; }
    }

    public class NuovaRigaComanda
    {
        public string Codice { get; set; }
        public int? Quantita { get; set; }
    }
}
