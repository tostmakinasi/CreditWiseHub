using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos
{
    public class RabbitMQDto
    {
        public string ExchangeName = "CreditWiseHubDirectExchange";
        public string Routing = "";
        public string QueueName = "";
    }
}
