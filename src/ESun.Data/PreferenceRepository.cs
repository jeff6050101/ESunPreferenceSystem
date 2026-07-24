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

        // ===================================================================
        // 範例：查詢喜好清單【需求 2】
        // 其餘方法請照此模式實作
        // ===================================================================
        public async Task<IEnumerable<PreferenceListItemDto>> GetListAsync(string userID)
        {
            // using 確保連線用完歸還連線池；Dapper 會自動開啟連線
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.QueryAsync<PreferenceListItemDto>(
                "usp_Preference_GetList",
                new 
                {
                    UserID = userID 
                },                 // 參數：匿名物件的屬性名須與 SP 參數同名
                commandType: CommandType.StoredProcedure // 關鍵：告訴 Dapper 這是 SP 不是 SQL 字串
            );
        }


        // ===================================================================
        // TODO：以下五個方法由你實作
        // ===================================================================

        // 提示：回傳多筆 → QueryAsync<T>
        //       SP 無參數時，第二個參數可省略
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            // using 確保連線用完歸還連線池；Dapper 會自動開啟連線
            using IDbConnection conn = _connectionFactory.CreateConnection();

            return await conn.QueryAsync<User>(
                "usp_User_GetList",
                commandType: CommandType.StoredProcedure
                );

            
        }

        // 提示：回傳單筆或 null → QueryFirstOrDefaultAsync<T>
        //       （不要用 QueryFirstAsync，那個查無資料時會拋例外）
        //       SP 參數：@SN、@UserID
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

        // 提示：SP 最後是 SELECT @NewSN AS SN，只回傳單一值 → ExecuteScalarAsync<int>
        //       SP 參數共 8 個：@UserID、@ProductName、@Price、@FeeRate、
        //                       @PurchaseQuantity、@Account、@TotalFee、@TotalAmount
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

        // 提示：SP 回傳 SELECT ? AS AffectedRows → ExecuteScalarAsync<int>
        //       比 Add 多一個 @SN（input.SN 是 int?，SP 需要 int，要處理轉換）
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

        // 提示：SP 參數：@SN、@UserID
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
