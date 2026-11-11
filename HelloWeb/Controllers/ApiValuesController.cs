using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HelloWeb.Controllers
{
    /// <summary>
    /// 모델 클래스
    /// </summary>
    public class Value
    {
        public int Id { get; set; }
        [MaxLength(5)]
        public string Text { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ApiValuesController : ControllerBase
    {
        // GET: api/<ApiValuesController>
        [HttpGet]
        public IEnumerable<Value> Get()
        {
            //return new string[] { "안녕하세요", "반갑습니다" };
            return new Value[] {
                new Value { Id = 1, Text = "안녕하세요" },
                new Value { Id = 2, Text = "반갑습니다" }
            };
        }

        // GET api/<ApiValuesController>/5
        [HttpGet("{id}")]
        public Value Get(int id)
        {
            //return "value";
            return new Value { Id = id, Text = $"넘어온 값: {id}" };
        }

        // POST api/<ApiValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] Value value)
        {
            return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
        }

        // PUT api/<ApiValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Value value)
        {
        }

        // DELETE api/<ApiValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
