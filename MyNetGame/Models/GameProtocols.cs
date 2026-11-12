using System.ComponentModel.DataAnnotations;

namespace MyNetGame.Models
{
    // ========================================================
    // 1. 유저 등록 프로토콜 (Protocol: 1102 -> -1102)
    // ========================================================
    public class UserAddRequest
    {
        public int Protocol { get; set; } = 1102;

        [Required(ErrorMessage = "UserId는 필수입니다.")]
        [StringLength(8, ErrorMessage = "UserId는 최대 8자입니다.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name은 필수입니다.")]
        [StringLength(20, ErrorMessage = "Name은 최대 20자입니다.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password는 필수입니다.")]
        [StringLength(20, ErrorMessage = "Password는 최대 20자입니다.")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserAddResponse
    {
        public int Protocol { get; set; } = -1102;

        /// <summary>
        /// 0: 등록 성공, 1: 중복 유저가 있음, 2: 등록 실패
        /// </summary>
        public int Result { get; set; }

        public string UserId { get; set; } = string.Empty;
    }

    // ========================================================
    // 2. 유저 정보 프로토콜 (Protocol: 1103 -> -1103)
    // ========================================================
    public class UserInfoRequest
    {
        public int Protocol { get; set; } = 1103;

        [Required(ErrorMessage = "UserId는 필수입니다.")]
        [StringLength(8, ErrorMessage = "UserId는 최대 8자입니다.")]
        public string UserId { get; set; } = string.Empty;
    }

    public class UserInfoResponse
    {
        public int Protocol { get; set; } = -1103;

        /// <summary>
        /// 0: 정보 가져오기 성공, 1: 정보 가져오기 실패
        /// </summary>
        public int Result { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
    }

    // ========================================================
    // 3. 레벨업 프로토콜 (Protocol: 1104 -> -1104)
    // ========================================================
    public class LevelupRequest
    {
        public int Protocol { get; set; } = 1104;

        [Required(ErrorMessage = "UserId는 필수입니다.")]
        [StringLength(8, ErrorMessage = "UserId는 최대 8자입니다.")]
        public string UserId { get; set; } = string.Empty;
    }

    public class LevelupResponse
    {
        public int Protocol { get; set; } = -1104;

        /// <summary>
        /// 0: 레벨업 성공, 1: 레벨업 실패
        /// </summary>
        public int Result { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    // ========================================================
    // 4. 골드 추가 프로토콜 (Protocol: 1105 -> -1105)
    // ========================================================
    public class AddGoldRequest
    {
        public int Protocol { get; set; } = 1105;

        [Required(ErrorMessage = "UserId는 필수입니다.")]
        [StringLength(8, ErrorMessage = "UserId는 최대 8자입니다.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gold는 필수입니다.")]
        public int Gold { get; set; }
    }

    public class AddGoldResponse
    {
        public int Protocol { get; set; } = -1105;

        /// <summary>
        /// 0: 추가 성공, 1: 추가 실패
        /// </summary>
        public int Result { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int Gold { get; set; }
    }
}
