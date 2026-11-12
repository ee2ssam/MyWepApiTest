using Microsoft.AspNetCore.Mvc;
using MyNetGame.Models;
using MyNetGame.Services;

namespace MyNetGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/user (전체 목록 조회)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "조회 중 오류 발생", error = ex.Message });
            }
        }

        // GET: api/user/test_user (단건 조회)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = $"user_id '{id}' 사용자를 찾을 수 없습니다." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "조회 중 오류 발생", error = ex.Message });
            }
        }

        // POST: api/user (사용자 등록)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserTbl user)
        {
            if (string.IsNullOrWhiteSpace(user.user_id))
            {
                return BadRequest(new { success = false, message = "user_id는 필수 항목입니다." });
            }

            try
            {
                var exists = await _userRepository.GetByIdAsync(user.user_id);
                if (exists != null)
                {
                    return Conflict(new { success = false, message = $"user_id '{user.user_id}'가 이미 존재합니다." });
                }

                var success = await _userRepository.InsertAsync(user);
                if (success)
                {
                    return CreatedAtAction(nameof(GetById), new { id = user.user_id }, user);
                }
                return BadRequest(new { success = false, message = "사용자 등록에 실패했습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "등록 중 오류 발생", error = ex.Message });
            }
        }

        // PUT: api/user/test_user (사용자 정보 수정)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserTbl user)
        {
            try
            {
                var existing = await _userRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new { success = false, message = $"user_id '{id}' 사용자가 존재하지 않습니다." });
                }

                user.user_id = id;
                var success = await _userRepository.UpdateAsync(user);
                if (success)
                {
                    return Ok(new { success = true, message = "수정되었습니다." });
                }
                return BadRequest(new { success = false, message = "수정에 실패했습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "수정 중 오류 발생", error = ex.Message });
            }
        }

        // DELETE: api/user/test_user (사용자 삭제)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existing = await _userRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new { success = false, message = $"user_id '{id}' 사용자가 존재하지 않습니다." });
                }

                var success = await _userRepository.DeleteAsync(id);
                if (success)
                {
                    return Ok(new { success = true, message = "삭제되었습니다." });
                }
                return BadRequest(new { success = false, message = "삭제에 실패했습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "삭제 중 오류 발생", error = ex.Message });
            }
        }
    }
}
