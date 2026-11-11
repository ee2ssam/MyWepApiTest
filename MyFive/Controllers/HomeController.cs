using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFive.Data;
using MyFive.Models;
using System.Diagnostics;

namespace MyFive.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Supabase DB 연결 테스트 및 조회 엔드포인트
        public async Task<IActionResult> DbTest()
        {
            try
            {
                bool canConnect = await _context.Database.CanConnectAsync();
                return Json(new
                {
                    success = canConnect,
                    databaseProvider = _context.Database.ProviderName,
                    message = canConnect 
                        ? "Supabase (PostgreSQL) 데이터베이스 연결에 성공했습니다!" 
                        : "Supabase 데이터베이스에 연결할 수 없습니다. 연결 문자열을 확인해주세요."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    databaseProvider = _context.Database.ProviderName,
                    message = "연결 중 오류가 발생했습니다.",
                    error = ex.Message
                });
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
