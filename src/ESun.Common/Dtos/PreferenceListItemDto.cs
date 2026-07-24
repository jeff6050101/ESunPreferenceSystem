namespace ESun.Common.Dtos
{
    /// <summary>
    /// 喜好清單查詢結果的單筆資料。
    /// 對應 usp_Preference_GetList / usp_Preference_GetById 的輸出欄位，
    /// 內容橫跨 LikeList、[User]、Product 三張表，故以 DTO 而非單一 Entity 表示。
    /// </summary>
    public class PreferenceListItemDto
    {
        public int SN { get; set; }

        // 來自 [User]
        public string UserID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        /// <summary>使用者聯絡電子信箱（需求 2 指定欄位）</summary>
        public string Email { get; set; } = string.Empty;

        // 來自 Product
        public int ProductNo { get; set; }
        /// <summary>產品名稱（需求 2 指定欄位）</summary>
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal FeeRate { get; set; }

        // 來自 LikeList
        public int PurchaseQuantity { get; set; }
        /// <summary>扣款帳號（需求 2 指定欄位）</summary>
        public string Account { get; set; } = string.Empty;
        /// <summary>總手續費用（需求 2 指定欄位）</summary>
        public decimal TotalFee { get; set; }
        /// <summary>預計扣款總金額（需求 2 指定欄位）</summary>
        public decimal TotalAmount { get; set; }
    }
}
