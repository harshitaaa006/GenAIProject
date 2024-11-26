using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SafeStreet.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<SafeStreetContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SafeStreetContext") ?? throw new InvalidOperationException("Connection string 'SafeStreetContext' not found.")));
builder.Services.AddConnections();
builder.Services.AddEndpointsApiExplorer();
builder.Configuration["MapboxApiKey"] = Environment.GetEnvironmentVariable("MAPBOX_API_KEY");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapFallbackToPage("/Search");

app.MapControllerRoute(
    name: "defalut",
    pattern: "{Controller=Home}/{action=Index}/{id?}");

    


app.Run();
