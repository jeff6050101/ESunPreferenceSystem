using Dapper;
using ESun.Common.Dtos;
using ESun.Common.Entities;
using System.Data;

namespace ESun.Data
{
    /// <summary>
    /// 喜好金融商品資料存取實作（Dapper + Stored Procedure）。
    ///
    /// 【防 SQL Injection】
    /// 所有呼叫皆指定 commandType: CommandType.StoredProcedure，並以匿名物件傳遞參數，
    /// Dapper 會將其轉為 SqlParameter。參數永遠被視為「值」而非可執行的 SQL 片段，
    /// 本檔案不存在任何字串拼接的 SQL。
    /// </summary>
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PreferenceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<PreferenceListItemDto>> GetListAsync(string userID)
        {
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.QueryAsync<PreferenceListItemDto>(
                "usp_Preference_GetList",
                new { UserID = userID },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.QueryAsync<User>(
                "usp_User_GetList",
                commandType: CommandType.StoredProcedure
                );
        }

        // 用 QueryFirstOrDefaultAsync 而非 QueryFirstAsync：查無資料時回傳 null 而非拋例外
        public async Task<PreferenceListItemDto?> GetByIdAsync(int sn, string userID)
        {
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.QueryFirstOrDefaultAsync<PreferenceListItemDto>(
                "usp_Preference_GetById",
                new
                {
                    SN = sn,
                    UserID = userID
                },
                commandType: CommandType.StoredProcedure
                );
        }

        // SP 以 SELECT @NewSN AS SN 回傳單一純量值，故用 ExecuteScalarAsync<int>
        public async Task<int> AddAsync(PreferenceInputDto input, decimal totalFee, decimal totalAmount)
        {
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.ExecuteScalarAsync<int>(
                "usp_Preference_Add",
                new { UserID = input.UserID, ProductName = input.ProductName, Price  = input.Price ,
                    FeeRate = input.FeeRate, PurchaseQuantity = input.PurchaseQuantity, Account = input.Account,
                    TotalFee = totalFee, TotalAmount = totalAmount
                },
                commandType: CommandType.StoredProcedure
                );
        }

        // SP 以 SELECT ... AS AffectedRows 回傳單一純量值，故同樣用 ExecuteScalarAsync<int>
        public async Task<int> UpdateAsync(PreferenceInputDto input, decimal totalFee, decimal totalAmount)
        {
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.ExecuteScalarAsync<int>(
                "usp_Preference_Update",
                new
                {
                    UserID = input.UserID,
                    ProductName = input.ProductName,
                    SN = input.SN ?? throw new ArgumentException("更新時 SN 不可為 null", nameof(input)),
                    Price = input.Price,
                    FeeRate = input.FeeRate,
                    PurchaseQuantity = input.PurchaseQuantity,
                    Account = input.Account,
                    TotalFee = totalFee,
                    TotalAmount = totalAmount
                },
                commandType: CommandType.StoredProcedure
                );
        }

        public async Task<int> DeleteAsync(int sn, string userID)
        {
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.ExecuteScalarAsync<int>(
                "usp_Preference_Delete",
                new
                {
                    SN = sn,
                    UserID = userID,

                },
                commandType: CommandType.StoredProcedure
                );
        }
    }
}
