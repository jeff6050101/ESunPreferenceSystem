using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ESun.Data
{
    /// <summary>
    /// SQL Server 連線工廠。
    /// 連線字串一律由 appsettings.json 讀取，不寫死在程式碼中。
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private const string ConnectionName = "ESunPreferenceDB";
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(ConnectionName)
                ?? throw new InvalidOperationException(
                    $"appsettings.json 缺少連線字串設定：ConnectionStrings:{ConnectionName}");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
