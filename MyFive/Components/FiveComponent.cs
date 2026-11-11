using Microsoft.AspNetCore.Mvc;
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
    /// 모델 클래스 : Supabase postgres의 five 테이블과 1:1 매핑
    /// </summary>
    public class Five
    {
        public int Id { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 인터페이스
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
    /// 데이터베이스 액세스 레이어 구현 클래스 (Supabase PostgreSQL / Dapper 기반)
    /// </summary>
    public class FiveRepository : IFiveRepository
    {
        private readonly string _connectionString;

        public FiveRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public List<Five> GetAll()
        {
            // Supabase PostgreSQL의 five 테이블의 모든 레코드 조회
            const string sql = "SELECT id, note FROM five ORDER BY id DESC;";
            using var db = CreateConnection();
            return db.Query<Five>(sql).ToList();
        }

        public Five? GetById(int id)
        {
            const string sql = "SELECT id, note FROM five WHERE id = @Id;";
            using var db = CreateConnection();
            return db.Query<Five>(sql, new { Id = id }).FirstOrDefault();
        }

        public void Add(Five five)
        {
            const string sql = "INSERT INTO five (note) VALUES (@Note);";
            using var db = CreateConnection();
            db.Execute(sql, new { Note = five.Note });
        }

        public void Update(Five five)
        {
            const string sql = "UPDATE five SET note = @Note WHERE id = @Id;";
            using var db = CreateConnection();
            db.Execute(sql, five);
        }

        public void Remove(int id)
        {
            const string sql = "DELETE FROM five WHERE id = @Id;";
            using var db = CreateConnection();
            db.Execute(sql, new { Id = id });
        }
    }

    /// <summary>
    /// Web API 컨트롤러 (/api/FiveService)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FiveServiceController : ControllerBase
    {
        private readonly IFiveRepository _repository;

        public FiveServiceController(IFiveRepository repository)
        {
            _repository = repository;
        }

        // GET: api/FiveService (Supabase five 테이블 전체 조회)
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
                return BadRequest(new { message = "five 테이블 조회 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        // GET: api/FiveService/5 (단건 조회)
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var five = _repository.GetById(id);
                if (five == null)
                {
                    return NotFound(new { message = $"{id}번 데이터를 찾을 수 없습니다." });
                }
                return Ok(five);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "조회 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        // POST: api/FiveService (데이터 추가)
        [HttpPost]
        public IActionResult Post([FromBody] Five five)
        {
            try
            {
                _repository.Add(five);
                return Ok(new { message = "데이터가 성공적으로 등록되었습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "등록 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        // PUT: api/FiveService/5 (데이터 수정)
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Five five)
        {
            try
            {
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound(new { message = $"{id}번 데이터가 없습니다." });
                }

                five.Id = id;
                _repository.Update(five);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "수정 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        // DELETE: api/FiveService/5 (데이터 삭제)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound(new { message = $"{id}번 데이터가 없습니다." });
                }

                _repository.Remove(id);
                return Ok(new { message = $"{id}번 데이터가 삭제되었습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "삭제 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }
}
