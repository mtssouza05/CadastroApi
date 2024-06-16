using CadastroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace CadastroApi.Controllers
{
    [ApiController]
    [Route("CadastroApi/[controller]")]
    public class CadastroController : Controller
    {
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("CadastrarUsuario")]
        public IActionResult Cadastro([FromBody] Usuario usuario)
        {

            try
            {
                var host = new ConnectionFactory() { HostName = "localhost"};

                using (var con = host.CreateConnection())
                using (var canal = con.CreateModel())
                {
                    canal.QueueDeclare(queue: "cadastroQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var mensagemJson = JsonConvert.SerializeObject(usuario);
                    var corpoMensagem = Encoding.UTF8.GetBytes(mensagemJson);

                    canal.BasicPublish(exchange: "", routingKey: "cadastroQueue", basicProperties: null, body: corpoMensagem);
                }

                return Ok("Cadastro envia para a fila");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"ERRO: {ex.Message}");
            }
        }

        [HttpGet("RetornaQueries")]
        public IActionResult RetornaQuery(string nomeArq)
        {
            try
            {
                var caminho = @"c:\CadastrosJson\" + nomeArq;

                if (System.IO.File.Exists(caminho))
                {
                    var json = System.IO.File.ReadAllText(caminho);
                    var querys = JsonConvert.DeserializeObject<List<string>>(json);
                    return Ok(querys);
                }

                return NotFound("Query não encontrada");

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"ERRO: {ex.Message}");
            }
        }
    }
}
