using CbClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<CbProvider>();
builder.Services.AddSingleton<CbProvider1>();

var app = builder.Build();

app.Services.GetRequiredService<CbProvider>();
app.Services.GetRequiredService<CbProvider1>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
