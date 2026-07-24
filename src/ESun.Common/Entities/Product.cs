namespace ESun.Common.Entities
{
    /// <summary>
    /// 金融商品，對應資料表 Product。
    /// </summary>
    public class Product
    {
        /// <summary>產品流水號（主鍵，IDENTITY）</summary>
        public int No { get; set; }

        /// <summary>產品名稱</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 產品價格。
        /// 對應 DECIMAL(18,4)，使用 decimal 以避免浮點誤差。
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 手續費率，以小數表示：0.03 = 3%。
        /// 資料庫端由 CK_Product_FeeRate 限制於 0 ~ 1。
        /// </summary>
        public decimal FeeRate { get; set; }
    }
}
