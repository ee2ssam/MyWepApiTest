using System.Data;
using Dapper;
using Npgsql;
using MyNetGame.Models;

namespace MyNetGame.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserTbl>> GetAllAsync();
        Task<UserTbl?> GetByIdAsync(string userId);
        Task<bool> InsertAsync(UserTbl user);
        Task<bool> UpdateAsync(UserTbl user);
        Task<bool> DeleteAsync(string userId);

        // 프로토콜 저장 프로시저 호출
        Task<int> LoginAsync(string userId, string password);
        Task<int> UserAddAsync(string userId, string name, string password);
        Task<UserInfoResponse> GetUserInfoAsync(string userId);
        Task<LevelupResponse> LevelUpAsync(string userId);
        Task<AddGoldResponse> AddGoldAsync(string userId, int gold);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        // 1. SELECT 전체 조회
        public async Task<IEnumerable<UserTbl>> GetAllAsync()
        {
            using var db = CreateConnection();
            const string sql = @"
                SELECT user_id, name, password, mobile, level, health, gold, m_date 
                FROM user_tbl 
                ORDER BY user_id;";
            return await db.QueryAsync<UserTbl>(sql);
        }

        // 2. SELECT 단건 조회
        public async Task<UserTbl?> GetByIdAsync(string userId)
        {
            using var db = CreateConnection();
            const string sql = @"
                SELECT user_id, name, password, mobile, level, health, gold, m_date 
                FROM user_tbl 
                WHERE user_id = @userId;";
            return await db.QueryFirstOrDefaultAsync<UserTbl>(sql, new { userId });
        }

        // 3. INSERT 추가
        public async Task<bool> InsertAsync(UserTbl user)
        {
            using var db = CreateConnection();
            const string sql = @"
                INSERT INTO user_tbl (user_id, name, password, mobile, level, health, gold, m_date) 
                VALUES (@user_id, @name, @password, @mobile, @level, @health, @gold, COALESCE(@m_date, CURRENT_DATE));";
            var affected = await db.ExecuteAsync(sql, user);
            return affected > 0;
        }

        // 4. UPDATE 수정
        public async Task<bool> UpdateAsync(UserTbl user)
        {
            using var db = CreateConnection();
            const string sql = @"
                UPDATE user_tbl 
                SET name = COALESCE(@name, name),
                    password = COALESCE(@password, password),
                    mobile = COALESCE(@mobile, mobile),
                    level = COALESCE(@level, level),
                    health = COALESCE(@health, health),
                    gold = COALESCE(@gold, gold),
                    m_date = COALESCE(@m_date, m_date)
                WHERE user_id = @user_id;";
            var affected = await db.ExecuteAsync(sql, user);
            return affected > 0;
        }

        // 5. DELETE 삭제
        public async Task<bool> DeleteAsync(string userId)
        {
            using var db = CreateConnection();
            const string sql = "DELETE FROM user_tbl WHERE user_id = @userId;";
            var affected = await db.ExecuteAsync(sql, new { userId });
            return affected > 0;
        }

        // 6. LOGIN 프로시저 (sp_login, Protocol 1101)
        public async Task<int> LoginAsync(string userId, string password)
        {
            using var db = CreateConnection();
            const string sql = "SELECT sp_login(@userId, @password);";
            return await db.ExecuteScalarAsync<int>(sql, new { userId, password });
        }

        // 7. 유저 등록 프로시저 (sp_user_add, Protocol 1102)
        public async Task<int> UserAddAsync(string userId, string name, string password)
        {
            using var db = CreateConnection();
            const string sql = "SELECT sp_user_add(@userId, @name, @password);";
            return await db.ExecuteScalarAsync<int>(sql, new { userId, name, password });
        }

        // 8. 플레이 유저 정보 프로시저 (sp_user_info, Protocol 1103)
        public async Task<UserInfoResponse> GetUserInfoAsync(string userId)
        {
            using var db = CreateConnection();
            const string sql = "SELECT result, user_id AS UserId, name, level, health, gold FROM sp_user_info(@userId);";
            var row = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { userId });

            if (row == null)
            {
                return new UserInfoResponse
                {
                    Protocol = -1103,
                    Result = 1,
                    UserId = userId
                };
            }

            return new UserInfoResponse
            {
                Protocol = -1103,
                Result = (int)row.result,
                UserId = (string)row.userid,
                Name = (string)row.name,
                Level = (int)row.level,
                Health = (int)row.health,
                Gold = (int)row.gold
            };
        }

        // 9. 레벨업 프로시저 (sp_level_up, Protocol 1104)
        public async Task<LevelupResponse> LevelUpAsync(string userId)
        {
            using var db = CreateConnection();
            const string sql = "SELECT result, user_id AS UserId, level FROM sp_level_up(@userId);";
            var row = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { userId });

            if (row == null)
            {
                return new LevelupResponse
                {
                    Protocol = -1104,
                    Result = 1,
                    UserId = userId,
                    Level = 0
                };
            }

            return new LevelupResponse
            {
                Protocol = -1104,
                Result = (int)row.result,
                UserId = (string)row.userid,
                Level = (int)row.level
            };
        }

        // 10. 골드 추가 프로시저 (sp_add_gold, Protocol 1105)
        public async Task<AddGoldResponse> AddGoldAsync(string userId, int gold)
        {
            using var db = CreateConnection();
            const string sql = "SELECT result, user_id AS UserId, gold FROM sp_add_gold(@userId, @gold);";
            var row = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { userId, gold });

            if (row == null)
            {
                return new AddGoldResponse
                {
                    Protocol = -1105,
                    Result = 1,
                    UserId = userId,
                    Gold = 0
                };
            }

            return new AddGoldResponse
            {
                Protocol = -1105,
                Result = (int)row.result,
                UserId = (string)row.userid,
                Gold = (int)row.gold
            };
        }
    }
}
