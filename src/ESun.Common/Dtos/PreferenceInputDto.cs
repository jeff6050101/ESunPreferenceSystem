namespace ESun.Common.Dtos
{
    /// <summary>
    /// 新增／更改喜好金融商品的輸入資料，由展示層傳入業務層。
    ///
    /// 注意：此 DTO 只承載「使用者輸入的原始資料」，
    /// TotalFee 與 TotalAmount 屬計算結果，由業務層負責產生後再傳給資料層，
    /// 不由展示層提供，避免金額被前端竄改。
    /// </summary>
    public class PreferenceInputDto
    {
        /// <summary>
        /// 喜好紀錄流水序號。
        /// 新增時為 null；更改時必填，用以定位要異動的紀錄。
        /// </summary>
        public int? SN { get; set; }

        /// <summary>使用者 ID</summary>
        public string UserID { get; set; } = string.Empty;

        // 產品資訊（寫入 Product 表）
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        /// <summary>手續費率，以小數表示：0.03 = 3%</summary>
        public decimal FeeRate { get; set; }

        // 喜好資訊（寫入 LikeList 表）
        public int PurchaseQuantity { get; set; }
        /// <summary>本筆指定的扣款帳號</summary>
        public string Account { get; set; } = string.Empty;
    }
}
