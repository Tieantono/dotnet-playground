using CbServer.Services;
using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue.RangeScan;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CouchbaseService>();
builder.Services.AddCouchbase(builder.Configuration.GetSection("Couchbase"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", async (CouchbaseService cb) =>
{
    IAsyncEnumerable<IScanResult> results = cb.RuleCollection.ScanAsync(
        new PrefixScan("")
        );

    var data = await results
    .Select(Q => new
    {
        Q.Id,
        Expression = Q.ContentAs<RuleViewModel>()
    })
    .ToListAsync();

    return Results.Ok(data);
});

app.MapPost("/", async (CouchbaseService cb, [FromBody] RuleViewModel form) =>
{
    var result = await cb.RuleCollection.InsertAsync(Guid.NewGuid().ToString(), new RuleViewModel
    {
        Expression = form.Expression
    });

    return Results.Ok(result);
});

app.Run();


public class RuleViewModel
{
    public string Expression { get; set; } = string.Empty;
}
