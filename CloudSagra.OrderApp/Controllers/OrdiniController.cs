using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CloudSagra.OrderApp.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace CloudSagra.OrderApp.Controllers
{
    public class OrdiniController : Controller
    {
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        // public IActionResult New(int numeroTavolo, NuovaRigaOrdine[] righe)
        public async Task<IActionResult> New([FromForm] NuovoOrdine nuovoOrdine)
        {
            using (var conn = new SqlConnection(""))
            {
                conn.Open();

                //var context = new OrdiniContext();
                var dataOrdine = DateTime.Now;

                foreach (var riga in nuovoOrdine.Righe)
                {
                    if (!riga.Quantita.HasValue) continue;
                    if (riga.Quantita.Value == 0) continue;

                    var rigaOrdine = new RigaOrdine
                    {
                        NumeroTavolo = nuovoOrdine.NumeroTavolo,
                        DataOrdine = dataOrdine,
                        Codice = riga.Codice,
                        Categoria = riga.Categoria,
                        Quantita = riga.Quantita.Value,
                        Prezzo = riga.Prezzo
                    };

                    conn.Execute("INSERT INTO RigheOrdini(NumeroTavolo, DataOrdine, Codice, Categoria, Quantita, Prezzo) VALUES (@NumeroTavolo, @DataOrdine, @Codice, @Categoria, @Quantita, @Prezzo)", rigaOrdine);

                    //context.RigheOrdini.Add(rigaOrdine);
                }

                conn.Close();
            }

            var primi = nuovoOrdine.Righe.Where(xx => xx.Categoria == "Primi").ToList();
            var secondi = nuovoOrdine.Righe.Where(xx => xx.Categoria == "Secondi").ToList();
            var bevande = nuovoOrdine.Righe.Where(xx => xx.Categoria == "Bevande").ToList();
            //var primi = new List<object>();
            //foreach(var riga in nuovoOrdine.Righe)
            //{
            //    if (riga.Categoria == "Primi")
            //        primi.Add(new { riga.Codice, riga.Quantita, nuovoOrdine.NumeroTavolo });
            //}

            await Send(primi, "Primi");
            await Send(secondi, "Secondi");
            await Send(bevande, "Bevande");

            return RedirectToAction("New");
        }

        private static async Task Send(List<NuovaRigaOrdine> righe, string nomeCoda)
        {
            if (righe == null) return;
            if (righe.Count == 0) return;

            var queueClient = new QueueClient("", nomeCoda);
            var json = JsonConvert.SerializeObject(righe);
            var bytes = Encoding.UTF8.GetBytes(json);
            var message = new Message(bytes);
            await queueClient.SendAsync(message);
        }


        public IActionResult TotaleVendutoPerPortata(DateTime data)
        {
            var domani = data.AddDays(1);

            var sql = "SELECT Categoria, SUM(Quantita) AS Quantita, SUM(Quantita*Prezzo) AS Totale FROM RigheOrdini WHERE DataOrdine>=@oggi AND DataOrdine < @domani GROUP BY Categoria";

            using (var conn = new SqlConnection(""))
            {
                conn.Open();

                var result = conn.Query<object>(sql, new { oggi = data, domani = domani });


                conn.Close();
            }
        }
    }
}