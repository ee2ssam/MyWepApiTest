using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyNetGame.Services;

namespace MyNetGame.Controllers
{
    [ApiController]
    public class PingController : ControllerBase
    {
        private static readonly DateTime StartTime = DateTime.UtcNow;
        private readonly ISupabaseDbService _dbService;

        public PingController(ISupabaseDbService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Render 콜드스타트 방지 및 헬스체크 핑 Web API
        /// GET /api/ping 또는 GET /healthz
        /// </summary>
        /// <param name="checkDb">true인 경우 Supabase DB 연결까지 함께 웜업(Warm-up) 체크</param>
        [HttpGet]
        [Route("api/ping")]
        [Route("api/health")]
        [Route("healthz")]
        public async Task<IActionResult> Ping([FromQuery] bool checkDb = false)
        {
            var uptime = DateTime.UtcNow - StartTime;

            var result = new Dictionary<string, object>
            {
                { "status", "ok" },
                { "message", "pong" },
                { "serverTimeUtc", DateTime.UtcNow },
                { "uptime", $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s" }
            };

            // DB 웜업 옵션
            if (checkDb)
            {
                var dbOk = await _dbService.CanConnectAsync();
                result.Add("database", dbOk ? "connected" : "disconnected");
            }

            return Ok(result);
        }
    }
}
