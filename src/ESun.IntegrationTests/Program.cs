using ESun.Common.Dtos;
using ESun.Data;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:ESunPreferenceDB"] =
            @"Server=.\SQLEXPRESS;Database=ESunPreferenceDB;Trusted_Connection=True;TrustServerCertificate=True"
    })
    .Build();

IPreferenceRepository repo = new PreferenceRepository(new SqlConnectionFactory(config));

const string USER_A = "A1236456789";   // 王小明
const string USER_B = "B2234567890";   // 李大華
int pass = 0, fail = 0;

void Check(string name, bool ok, string detail)
{
    Console.WriteLine($"  [{(ok ? "PASS" : "FAIL")}] {name}  {detail}");
    if (ok) pass++; else fail++;
}

Console.WriteLine("=== 1. GetUsersAsync ===");
var users = (await repo.GetUsersAsync()).ToList();
Check("取得使用者", users.Count == 3, $"共 {users.Count} 筆");
Check("欄位對應", users.All(u => !string.IsNullOrEmpty(u.UserName) && !string.IsNullOrEmpty(u.Account)),
      $"首筆 = {users[0].UserID} / {users[0].UserName} / {users[0].Account}");

Console.WriteLine("=== 2. GetListAsync ===");
var listA = (await repo.GetListAsync(USER_A)).ToList();
Check("只回傳該使用者", listA.All(x => x.UserID == USER_A), $"共 {listA.Count} 筆");
Check("JOIN 欄位對應", listA.All(x => !string.IsNullOrEmpty(x.ProductName) && !string.IsNullOrEmpty(x.Email)),
      $"首筆 = {listA[0].ProductName} / {listA[0].Email} / 總額 {listA[0].TotalAmount}");

Console.WriteLine("=== 3. GetByIdAsync ===");
var one = await repo.GetByIdAsync(listA[0].SN, USER_A);
Check("取得自己的紀錄", one != null && one.SN == listA[0].SN, $"SN={one?.SN}");
var listB = (await repo.GetListAsync(USER_B)).ToList();
var stolen = await repo.GetByIdAsync(listB[0].SN, USER_A);
Check("越權存取被擋", stolen == null, $"以 A 身分讀 B 的 SN={listB[0].SN} → {(stolen == null ? "null" : "取得了！")}");

Console.WriteLine("=== 4. AddAsync ===");
var input = new PreferenceInputDto
{
    UserID = USER_A, ProductName = "整合測試基金", Price = 50m, FeeRate = 0.02m,
    PurchaseQuantity = 100, Account = "1111999666"
};
int newSn = await repo.AddAsync(input, 100m, 5100m);
Check("回傳新 SN", newSn > 0, $"SN={newSn}");
var added = await repo.GetByIdAsync(newSn, USER_A);
Check("資料正確寫入", added != null && added.ProductName == "整合測試基金" && added.TotalAmount == 5100m,
      $"{added?.ProductName} / 手續費 {added?.TotalFee} / 總額 {added?.TotalAmount}");

Console.WriteLine("=== 5. UpdateAsync ===");
input.SN = newSn;
input.ProductName = "整合測試基金-改名";
input.Price = 60m;
int uRows = await repo.UpdateAsync(input, 360m, 12360m);
Check("回傳 AffectedRows=1", uRows == 1, $"AffectedRows={uRows}");
var updated = await repo.GetByIdAsync(newSn, USER_A);
Check("兩表皆已更新", updated != null && updated.ProductName == "整合測試基金-改名" && updated.Price == 60m,
      $"{updated?.ProductName} / 單價 {updated?.Price}");
int uSteal = await repo.UpdateAsync(
    new PreferenceInputDto { SN = listB[0].SN, UserID = USER_A, ProductName = "駭客", Price = 1m,
                             FeeRate = 0.01m, PurchaseQuantity = 1, Account = "0" }, 0m, 1m);
Check("越權更新被擋", uSteal == 0, $"AffectedRows={uSteal}");

Console.WriteLine("=== 6. DeleteAsync ===");
int dSteal = await repo.DeleteAsync(listB[0].SN, USER_A);
Check("越權刪除被擋", dSteal == 0, $"AffectedRows={dSteal}");
int dRows = await repo.DeleteAsync(newSn, USER_A);
Check("刪除自己的紀錄", dRows == 1, $"AffectedRows={dRows}");
var afterDel = await repo.GetByIdAsync(newSn, USER_A);
Check("確實刪除", afterDel == null, afterDel == null ? "查無資料" : "仍存在！");
int dAgain = await repo.DeleteAsync(999999, USER_A);
Check("刪除不存在的 SN", dAgain == 0, $"AffectedRows={dAgain}");

Console.WriteLine();
Console.WriteLine($"===== 結果：{pass} 通過 / {fail} 失敗 =====");
