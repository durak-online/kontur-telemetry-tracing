using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using System.Diagnostics;
using tracing_test_service;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ABOBA_3_SPIDOMETER_3.0"))
    .WithTracing()
    .AddOtelTracing();
builder.Services.AddSingleton<SomeWorker>();

var activitySource = new ActivitySource("ABOBA_3_SPIDOMETER_3.0");
builder.Services.AddSingleton(activitySource);

var app = builder.Build();

app.MapGet("/", (
    [FromServices] ActivitySource activitySource,
    [FromServices] SomeWorker worker) =>
{
    using var activity = activitySource.StartActivity("GOODWorld");

    activity?.AddTag("aboba.tag", "spidometer");
    activity?.AddTag("worker.name", nameof(SomeWorker));

    worker.DoSomeWork();

    return "Hello World!";
});

app.Run();