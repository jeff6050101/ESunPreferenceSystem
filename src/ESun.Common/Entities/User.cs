namespace ESun.Common.Entities
{
    /// <summary>
    /// 使用者，對應資料表 [User]。
    /// </summary>
    public class User
    {
        /// <summary>使用者 ID（主鍵）</summary>
        public string UserID { get; set; } = string.Empty;

        /// <summary>使用者名稱</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>聯絡電子郵件</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>預設扣款帳號</summary>
        public string Account { get; set; } = string.Empty;
    }
}
