using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared;

namespace Server
{
    public class ClientComponent : Component
    {
        public int ID { get; set; } = 0;
        public Queue<ClientRequest> Requests { get; set; }

        public ClientComponent()
        {
            Requests = new Queue<ClientRequest>();
        }

        public void Reset()
        {
            ID = 0;
            Requests.Clear();
        }
    }
}
