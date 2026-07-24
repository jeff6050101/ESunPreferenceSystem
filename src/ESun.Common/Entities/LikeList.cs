namespace ESun.Common.Entities
{
    /// <summary>
    /// 喜好清單，對應資料表 LikeList。
    /// </summary>
    public class LikeList
    {
        /// <summary>流水序號（主鍵，IDENTITY）</summary>
        public int SN { get; set; }

        /// <summary>使用者 ID（外鍵 → [User].UserID）</summary>
        public string UserID { get; set; } = string.Empty;

        /// <summary>產品流水號（外鍵 → Product.No）</summary>
        public int ProductNo { get; set; }

        /// <summary>購買數量</summary>
        public int PurchaseQuantity { get; set; }

        /// <summary>
        /// 本筆指定的扣款帳號。
        /// 與 [User].Account 各自獨立：後者為預設值，此欄為建立當下的選擇快照。
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 總手續費（台幣）。建立當下計算後的快照，不隨產品日後改價而變動。
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 預計扣款總金額。建立當下計算後的快照。
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
