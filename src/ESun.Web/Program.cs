using ESun.Business;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 註冊業務層（內部一併帶入資料層）—— Web 不需直接參考 ESun.Data
builder.Services.AddBusinessLayer();

// Session：本系統無登入機制，以 Session 保存「目前操作的使用者」，模擬登入狀態
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;     // 禁止 JavaScript 讀取此 Cookie（防 XSS 竊取）
    options.Cookie.IsEssential = true;  // 必要性 Cookie，不受同意政策影響
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        // 必須在 UseRouting 之後、對應端點之前
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Preference}/{action=Index}/{id?}");

app.Run();
