using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Messaging.Configuration
{
    public class RabbitMqSettings
    {
        public const string SectionName = "RabbitMQ";

        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "ball.com.exchange";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public string ServiceName { get; set; } = "";
    }
}
