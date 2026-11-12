using System.Data;
using Dapper;
using Npgsql;

namespace MyNetGame.Services
{
    public class FiveItem
    {
        public int Id { get; set; }
        public string? Note { get; set; }
    }

    public interface ISupabaseDbService
    {
        Task<bool> CanConnectAsync();
        Task<IEnumerable<FiveItem>> GetFivesAsync();
        Task<int> AddFiveAsync(string note);
        Task<IEnumerable<string>> GetTablesAsync();
    }

    public class SupabaseDbService : ISupabaseDbService
    {
        private readonly string _connectionString;

        public SupabaseDbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<bool> CanConnectAsync()
        {
            try
            {
                using var db = CreateConnection();
                var result = await db.ExecuteScalarAsync<int>("SELECT 1;");
                return result == 1;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<FiveItem>> GetFivesAsync()
        {
            using var db = CreateConnection();
            const string sql = "SELECT id, note FROM five ORDER BY id DESC;";
            return await db.QueryAsync<FiveItem>(sql);
        }

        public async Task<int> AddFiveAsync(string note)
        {
            using var db = CreateConnection();
            const string sql = "INSERT INTO five (note) VALUES (@note) RETURNING id;";
            return await db.ExecuteScalarAsync<int>(sql, new { note });
        }

        public async Task<IEnumerable<string>> GetTablesAsync()
        {
            using var db = CreateConnection();
            const string sql = @"
                SELECT table_name 
                FROM information_schema.tables 
                WHERE table_schema = 'public' 
                ORDER BY table_name;";
            return await db.QueryAsync<string>(sql);
        }
    }
}
