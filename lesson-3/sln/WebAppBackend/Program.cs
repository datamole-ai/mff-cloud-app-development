using Microsoft.AspNetCore.Mvc;

using WebAppBackend.Clients;

using Transport = WebAppBackend.Models.Transport;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<StatsReporterClient>();
builder.Services.AddHttpClient(StatsReporterClient.HttpClientName, client =>
{
    client.BaseAddress = new(builder.Configuration["StatsReporter:FunctionHostUrl"] 
                             ?? throw new InvalidOperationException("FunctionHostUrl is not configured"));
    client.DefaultRequestHeaders.Add("x-functions-key", builder.Configuration["StatsReporter:FunctionHostKey"]
                             ?? throw new InvalidOperationException("FunctionHostKey is not configured"));
});
builder.Services.AddSingleton<DeviceDirectoryClientMock>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/transports",
        async (
            [FromQuery(Name = "date")] string dateQuery,
            [FromQuery] string facilityId,
            [FromQuery] string parcelId,
            [FromServices] StatsReporterClient reporterClient,
            [FromServices] DeviceDirectoryClientMock deviceDirectoryClientMock) =>
    {
        if (!DateOnly.TryParseExact(dateQuery, "yyyy-MM-dd", out var date))
        {
            return Results.BadRequest();
        }
        
        var transport = await reporterClient.FindTransportAsync(date, facilityId, parcelId);
        
        if (transport is null)
        {
            return Results.NotFound();
        }

        var device = await deviceDirectoryClientMock.GetDeviceAsync(transport.DeviceId);
        
        return Results.Ok(new {Transport = transport, Device = device});
    })
    .WithName("GetTransport")
    .WithOpenApi();


app.Run();
