using Microsoft.AspNetCore.Mvc;
using MoneyMinder.Controllers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MoneyMinder.ControllerAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogiApiController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        // 建構子注入 ILogger<HomeController>
        public LogiApiController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // GET: api/<LogiApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            // 寫一筆「訊息等級」的日誌
            _logger.LogInformation("HomeController.Index 被呼叫，當前時間：{Now}", DateTime.Now);
            _logger.LogError("執行 Privacy 時發生例外");
            return new string[] { "value1", "value2" };
        }

        // GET api/<LogiApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LogiApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LogiApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LogiApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
