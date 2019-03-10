using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using serverless.Hubs;

namespace serverless.test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private ConcurrentBag<string> values = new ConcurrentBag<string>(){
            "value1","value2"
        };
        private readonly IHubClients _hubClients;

        public ValuesController(IHubClients hubClients)
        {
            this._hubClients = hubClients;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return values;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            List<string> list = values.ToList();
            var val = list.Count >= id ? list[id] : "";


            return val;
        }

        public class Input

        {
            public string Value { get; set; }
        }

        // POST api/values
        [HttpPost]
        public async Task PostAsync([FromBody] Input value)
        {
            //values.Add(value.Value);

            await TestHub.SendValueAsync(JsonConvert.SerializeObject(new
            {
                action = "echo",
                data = value
            }), _hubClients);

            return;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
