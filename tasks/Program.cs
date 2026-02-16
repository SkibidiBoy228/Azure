using Microsoft.EntityFrameworkCore;
using System;
using tasks.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["AzureTranslator:Key"] =
    Environment.GetEnvironmentVariable("AZURE_TRANSLATOR_KEY");

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<CosmosService>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Translator}/{action=Index}/{id?}");

app.Run();
