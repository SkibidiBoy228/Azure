var builder = WebApplication.CreateBuilder(args);

builder.Configuration["AzureTranslator:Key"] =
    Environment.GetEnvironmentVariable("AZURE_TRANSLATOR_KEY");

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

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

// 🔹 Session должен быть ПОСЛЕ UseRouting и ДО UseEndpoints
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Translator}/{action=Index}/{id?}");

app.Run();
