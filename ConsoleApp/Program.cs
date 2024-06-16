using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using CadastroConsumidor.Models;
using CadastroConsumidor.Services;

namespace CadastroConsumidor
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = new ConnectionFactory() { HostName = "localhost" };

            using (var con = host.CreateConnection())
            using (var canal = con.CreateModel())
            {
                canal.QueueDeclare(queue: "cadastroQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumidor = new EventingBasicConsumer(canal);
                consumidor.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Usuario? usuario = JsonConvert.DeserializeObject<Usuario>(message); //pode ser nulo

                    var query = $"INSERT INTO Users (User, Password, Email) VALUES ('{usuario.User}', '{usuario.Password}', '{usuario.Email}');";
                    Uteis.SalvarQuery(query);
                };

                canal.BasicConsume(queue: "cadastroQueue", autoAck: true, consumer: consumidor);

                Console.ReadLine();
            }
        }
    }
}