using Microsoft.AspNetCore.Mvc;
using MyFive.Data;
using System.Data;
using Npgsql;
using Dapper;

namespace MyFive.Components
{
    /// <summary>
    /// 빈 클래스
    /// </summary>
    public class FiveComponent
    {
    }

    /// <summary>
    /// 모델클래스 : Supabase postgres fiveTable과 1:1 매핑 설계
    /// </summary>
    public class Five
    {
        public int Id { get; set; }
        public string? note { get; set; }
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 인터페이스
    /// 기능 정의
    /// </summary>
    public interface IFiveRepository
    {
        List<Five> GetAll();
        Five? GetById(int id);
        void Add(Five five);
        void Update(Five five);
        void Remove(int id);
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 구현 클래스 (Supabase PostgreSQL)
    /// </summary>
    public class FiveRepository : IFiveRepository
    {
        private readonly string _connectionString;

        public FiveRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public void Add(Five five)
        {
            Five model = new Five() { note = five.note };
            string sql = "insert into five (note) values (@note)";
            using var db = CreateConnection();
            db.Execute(sql, model);
        }

        public List<Five> GetAll()
        {
            // 데이터베이스에서 fiveTable의 모든 레코드 조회 로직 구현
            string sql = "select id, note from five order by id desc";
            using var db = CreateConnection();
            return db.Query<Five>(sql).ToList();
        }

        public Five? GetById(int id)
        {
            string sql = "select id, note from five where id = @Id";
            using var db = CreateConnection();
            return db.Query<Five>(sql, new { Id = id }).FirstOrDefault();
        }

        public void Remove(int id)
        {
            string sql = "delete from five where id = @Id";
            using var db = CreateConnection();
            db.Execute(sql, new { Id = id });
        }

        public void Update(Five five)
        {
            string sql = "update five set note = @note where id = @Id";
            using var db = CreateConnection();
            db.Execute(sql, five);
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FiveServiceController : ControllerBase
    {
        private readonly IFiveRepository _repository;

        //생성자
        public FiveServiceController(IFiveRepository repository)
        {
            _repository = repository;
        }

        // GET: api/FiveService
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var fives = _repository.GetAll();
                return Ok(fives);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }            
        }

        // GET api/FiveService/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var five = _repository.GetById(id);
                if (five == null)
                {
                    return NotFound(new { message = $"{id}번 데이터가 없습니다" });
                }
                return Ok(five);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/FiveService
        [HttpPost]
        public IActionResult Post([FromBody] Five five)
        {
            try
            {
                _repository.Add(five);
                return Ok(new { message = "등록 성공" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/FiveService/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Five five)
        {
            try
            {
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound(new { message = $"{id}번 데이터가 없습니다" });
                }

                five.Id = id;
                _repository.Update(five);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/FiveService/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound(new { message = $"{id}번 데이터가 없습니다" });
                }

                _repository.Remove(id);
                return Ok(new { message = $"{id}번 데이터가 삭제되었습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
