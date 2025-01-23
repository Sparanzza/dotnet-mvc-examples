using Microsoft.EntityFrameworkCore;
using MVCInventarios.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<InventariosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventariosContext") ?? throw new InvalidOperationException("Connection string 'InventariosContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else {
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<InventariosContext>();
    await context.Database.EnsureCreatedAsync();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
