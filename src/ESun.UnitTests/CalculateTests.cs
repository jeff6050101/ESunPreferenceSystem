using ESun.Business;

namespace ESun.UnitTests;

/// <summary>
/// 業務層純計算邏輯 <see cref="PreferenceService.Calculate"/> 的單元測試。
/// 不碰資料庫，任何環境皆可執行。
/// </summary>
public class CalculateTests
{
    public static IEnumerable<object[]> 一般案例 => new[]
    {
        //            價格,    數量,  費率,    預期手續費,  預期總額
        new object[] { 45.20m, 1000, 0.015m,  678.00m,   45878.00m },
        new object[] { 25.80m,  500, 0.03m,   387.00m,   13287.00m },
        new object[] { 15.60m, 2000, 0.02m,   624.00m,   31824.00m },
    };

    // 手續費 = 價格 × 數量 × 費率；總額 = 價格 × 數量 + 手續費
    [Theory]
    [MemberData(nameof(一般案例))]
    public void 一般案例_手續費與總額正確(
        decimal price, int quantity, decimal feeRate, decimal expectedFee, decimal expectedAmount)
    {
        var (fee, amount) = PreferenceService.Calculate(price, quantity, feeRate);

        Assert.Equal(expectedFee, fee);
        Assert.Equal(expectedAmount, amount);
    }

    // ⭐ 四捨五入必須「逢五進位」(AwayFromZero)，不可用 .NET 預設的銀行家捨入，否則會少收手續費。
    //    33.33 × 3 × 0.015 = 1.49985 → 進位到小數 4 位應為 1.4999（銀行家捨入會變 1.4998）
    [Fact]
    public void 第四位小數逢五_應進位而非銀行家捨入()
    {
        var (fee, _) = PreferenceService.Calculate(33.33m, 3, 0.015m);

        Assert.Equal(1.4999m, fee);   // 若得到 1.4998m 代表用了銀行家捨入 → 少收手續費
    }

    // 費率為 0 → 免手續費，總額等於小計
    [Fact]
    public void 費率為零_免收手續費()
    {
        var (fee, amount) = PreferenceService.Calculate(100m, 5, 0m);

        Assert.Equal(0m, fee);
        Assert.Equal(500m, amount);
    }

    // 不變式：總額恆等於「小計 + 手續費」
    [Fact]
    public void 總額_恆為小計加手續費()
    {
        var (fee, amount) = PreferenceService.Calculate(33.33m, 3, 0.015m);

        Assert.Equal(33.33m * 3 + fee, amount);
    }
}
