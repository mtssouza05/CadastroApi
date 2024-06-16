using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroConsumidor.Services
{
    public static class Uteis
    {

        public static void SalvarQuery(string query)
        {
            string diretorio = @"C:\CadastrosJson";
            string caminho = Path.Combine(diretorio, "queries.json");

            List<string>? queries = new List<string>();

            if (System.IO.File.Exists(caminho))
            {
                var json = System.IO.File.ReadAllText(caminho);
                queries = JsonConvert.DeserializeObject<List<string>>(json);
            }

            queries.Add(query);
            System.IO.File.WriteAllText(caminho, JsonConvert.SerializeObject(queries, Formatting.Indented));
        }


    }
}
