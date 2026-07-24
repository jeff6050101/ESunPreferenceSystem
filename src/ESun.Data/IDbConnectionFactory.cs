using System.Data;

namespace ESun.Data
{
    /// <summary>
    /// 資料庫連線工廠。
    /// 以介面對外，讓 Repository 不需知道實際使用哪一種資料庫，
    /// 也便於單元測試時替換為假物件。
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 建立一個【尚未開啟】的連線。
        /// 呼叫端負責以 using 管理生命週期，確保連線歸還至連線池。
        /// </summary>
        IDbConnection CreateConnection();
    }
}
