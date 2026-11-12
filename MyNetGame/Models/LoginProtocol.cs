using System.ComponentModel.DataAnnotations;

namespace MyNetGame.Models
{
    /// <summary>
    /// 로그인 요청 파라미터 (Protocol: 1101)
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 프로토콜 번호 (1101)
        /// </summary>
        public int Protocol { get; set; } = 1101;

        /// <summary>
        /// 유저 아이디 (최대 8자)
        /// </summary>
        [Required(ErrorMessage = "UserId는 필수입니다.")]
        [StringLength(8, ErrorMessage = "UserId는 최대 8자입니다.")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 비밀번호 (최대 20자)
        /// </summary>
        [Required(ErrorMessage = "Password는 필수입니다.")]
        [StringLength(20, ErrorMessage = "Password는 최대 20자입니다.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 로그인 응답 파라미터 (Protocol: -1101)
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// 프로토콜 번호 (-1101)
        /// </summary>
        public int Protocol { get; set; } = -1101;

        /// <summary>
        /// 결과 상태 코드
        /// 0: 로그인 성공
        /// 1: 유저 아이디가 없음
        /// 2: 로그인 실패 (비밀번호 불일치)
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 유저 아이디
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}
