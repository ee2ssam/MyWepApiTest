using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyNetGame.Models;
using MyNetGame.Services;

namespace MyNetGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISupabaseDbService _dbService;

        public HomeController(ILogger<HomeController> logger, ISupabaseDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Supabase DB 연결 확인 엔드포인트
        [HttpGet]
        public async Task<IActionResult> CheckDb()
        {
            try
            {
                bool canConnect = await _dbService.CanConnectAsync();
                return Json(new
                {
                    success = canConnect,
                    message = canConnect
                        ? "Supabase (PostgreSQL) 데이터베이스 연결에 성공했습니다!"
                        : "Supabase 데이터베이스에 연결할 수 없습니다."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Supabase의 테이블 목록 조회
        [HttpGet]
        public async Task<IActionResult> GetTables()
        {
            try
            {
                var tables = await _dbService.GetTablesAsync();
                return Json(new { success = true, tables });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // five 테이블 목록 조회
        [HttpGet]
        public async Task<IActionResult> GetFives()
        {
            try
            {
                var items = await _dbService.GetFivesAsync();
                return Json(new { success = true, items });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // five 테이블 데이터 추가
        [HttpPost]
        public async Task<IActionResult> AddFive([FromBody] FiveItem model)
        {
            if (string.IsNullOrWhiteSpace(model.Note))
            {
                return BadRequest(new { success = false, message = "Note 내용을 입력해주세요." });
            }

            try
            {
                int newId = await _dbService.AddFiveAsync(model.Note);
                return Json(new { success = true, id = newId, message = "데이터가 성공적으로 등록되었습니다." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
