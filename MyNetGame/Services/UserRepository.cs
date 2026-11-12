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
                SELECT user_id, password, mobile, level, health, gold, m_date 
                FROM user_tbl 
                ORDER BY user_id;";
            return await db.QueryAsync<UserTbl>(sql);
        }

        // 2. SELECT 단건 조회
        public async Task<UserTbl?> GetByIdAsync(string userId)
        {
            using var db = CreateConnection();
            const string sql = @"
                SELECT user_id, password, mobile, level, health, gold, m_date 
                FROM user_tbl 
                WHERE user_id = @userId;";
            return await db.QueryFirstOrDefaultAsync<UserTbl>(sql, new { userId });
        }

        // 3. INSERT 추가
        public async Task<bool> InsertAsync(UserTbl user)
        {
            using var db = CreateConnection();
            const string sql = @"
                INSERT INTO user_tbl (user_id, password, mobile, level, health, gold, m_date) 
                VALUES (@user_id, @password, @mobile, @level, @health, @gold, COALESCE(@m_date, CURRENT_DATE));";
            var affected = await db.ExecuteAsync(sql, user);
            return affected > 0;
        }

        // 4. UPDATE 수정
        public async Task<bool> UpdateAsync(UserTbl user)
        {
            using var db = CreateConnection();
            const string sql = @"
                UPDATE user_tbl 
                SET password = COALESCE(@password, password),
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
    }
}
