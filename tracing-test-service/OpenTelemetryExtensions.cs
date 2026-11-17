using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace tracing_test_service;

public static class OpenTelemetryExtensions
{
    public static OpenTelemetryBuilder AddOtelTracing(this OpenTelemetryBuilder openTelemetryBuilder) => openTelemetryBuilder
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri("http://localhost:4318/v1/traces");
                    otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
                    otlp.ExportProcessorType = ExportProcessorType.Simple;
                })
                .ConfigureResource(rb =>
                {
                    var name = typeof(Program).Assembly.GetName();
                    rb.AddService(
                        serviceName: "ABOBA_3_SPIDOMETER_3.0",
                        serviceVersion: name.Version!.ToString(),
                        autoGenerateServiceInstanceId: true);
                    rb.AddEnvironmentVariableDetector();
                })
                .AddSource("ABOBA_3_SPIDOMETER_3.0")
                .AddHttpClientInstrumentation();
        });
}