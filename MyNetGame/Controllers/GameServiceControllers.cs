using Microsoft.AspNetCore.Mvc;
using MyNetGame.Models;
using MyNetGame.Services;

namespace MyNetGame.Controllers
{
    // ========================================================
    // 1. 유저등록 서비스 (Protocol: 1102 -> -1102)
    // 서비스 URL: POST /api/UserAddService
    // ========================================================
    [ApiController]
    public class UserAddServiceController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserAddServiceController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("api/UserAddService")]
        [Route("api/useradd")]
        public async Task<IActionResult> Post([FromBody] UserAddRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Ok(new UserAddResponse
                {
                    Protocol = -1102,
                    Result = 2, // 등록 실패
                    UserId = request.UserId ?? string.Empty
                });
            }

            try
            {
                int resultCode = await _userRepository.UserAddAsync(request.UserId, request.Name ?? string.Empty, request.Password);

                return Ok(new UserAddResponse
                {
                    Protocol = -1102,
                    Result = resultCode, // 0: 성공, 1: 중복 유저, 2: 실패
                    UserId = request.UserId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Protocol = -1102,
                    Result = 2,
                    UserId = request.UserId,
                    Error = ex.Message
                });
            }
        }
    }

    // ========================================================
    // 2. 플레이 유저 정보 서비스 (Protocol: 1103 -> -1103)
    // 서비스 URL: POST /api/UserInfoService
    // ========================================================
    [ApiController]
    public class UserInfoServiceController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserInfoServiceController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("api/UserInfoService")]
        [Route("api/userinfo")]
        public async Task<IActionResult> Post([FromBody] UserInfoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Ok(new UserInfoResponse
                {
                    Protocol = -1103,
                    Result = 1, // 실패
                    UserId = string.Empty
                });
            }

            try
            {
                var response = await _userRepository.GetUserInfoAsync(request.UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Protocol = -1103,
                    Result = 1,
                    UserId = request.UserId,
                    Error = ex.Message
                });
            }
        }
    }

    // ========================================================
    // 3. 레벨업 서비스 (Protocol: 1104 -> -1104)
    // 서비스 URL: POST /api/LevelupService
    // ========================================================
    [ApiController]
    public class LevelupServiceController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public LevelupServiceController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("api/LevelupService")]
        [Route("api/levelup")]
        public async Task<IActionResult> Post([FromBody] LevelupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Ok(new LevelupResponse
                {
                    Protocol = -1104,
                    Result = 1, // 실패
                    UserId = string.Empty
                });
            }

            try
            {
                var response = await _userRepository.LevelUpAsync(request.UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Protocol = -1104,
                    Result = 1,
                    UserId = request.UserId,
                    Error = ex.Message
                });
            }
        }
    }

    // ========================================================
    // 4. 골드추가 서비스 (Protocol: 1105 -> -1105)
    // 서비스 URL: POST /api/AddGoldService
    // ========================================================
    [ApiController]
    public class AddGoldServiceController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AddGoldServiceController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("api/AddGoldService")]
        [Route("api/addgold")]
        public async Task<IActionResult> Post([FromBody] AddGoldRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Ok(new AddGoldResponse
                {
                    Protocol = -1105,
                    Result = 1, // 실패
                    UserId = string.Empty
                });
            }

            try
            {
                var response = await _userRepository.AddGoldAsync(request.UserId, request.Gold);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Protocol = -1105,
                    Result = 1,
                    UserId = request.UserId,
                    Error = ex.Message
                });
            }
        }
    }
}
