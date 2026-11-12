using Microsoft.AspNetCore.Mvc;
using MyNetGame.Models;
using MyNetGame.Services;

namespace MyNetGame.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 로그인 프로토콜 Web API (Protocol: 1101 -> -1101)
        /// POST /api/login 또는 POST /api/user/login
        /// </summary>
        [HttpPost]
        [Route("api/login")]
        [Route("api/user/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 유효성 검사 (UserId, Password 필수)
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Ok(new LoginResponse
                {
                    Protocol = -1101,
                    Result = 1, // 유저 아이디가 없음
                    UserId = string.Empty
                });
            }

            try
            {
                // Supabase PostgreSQL의 sp_login 저장 프로시저 호출
                int resultCode = await _userRepository.LoginAsync(request.UserId, request.Password ?? string.Empty);

                var response = new LoginResponse
                {
                    Protocol = -1101,
                    Result = resultCode,
                    UserId = request.UserId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Protocol = -1101,
                    Result = -1,
                    UserId = request.UserId,
                    Error = ex.Message
                });
            }
        }
    }
}
