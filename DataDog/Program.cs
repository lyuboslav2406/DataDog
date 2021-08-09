using Datadog.Trace;
using Serilog.Context;
using StatsdClient;
using System;

namespace DataDog
{
    public class Program
    {
        static void Main(string[] args)
        {
            var dogstatsdConfig = new StatsdConfig
            {
                StatsdServerName = "127.0.0.1",
                StatsdPort = 8125,
            };

            var dogStatsdService = new DogStatsdService();
            dogStatsdService.Configure(dogstatsdConfig);
            var random = new Random(0);

            // there must be spans started and active before this block.
            using (LogContext.PushProperty("CloudConnector_dd_env", CorrelationIdentifier.Env))
            using (LogContext.PushProperty("CloudConnector_dd_service", CorrelationIdentifier.Service))
            using (LogContext.PushProperty("CloudConnector_dd_version", CorrelationIdentifier.Version))
            using (LogContext.PushProperty("CloudConnector_dd_trace_id", CorrelationIdentifier.TraceId.ToString()))
            using (LogContext.PushProperty("CloudConnector_dd_span_id", CorrelationIdentifier.SpanId.ToString()))
            {
                // Log something
            }

            dogStatsdService.ServiceCheck("Service.check.CloudConnector", 0, message: "Application is ON.", tags: new[] { "environment:dev" });

            for (int i = 0; i < 100; i++)
            {
                dogStatsdService.Increment("CloudConnector_metric.increment", tags: new[] { "environment:dev" });
                //dogStatsdService.Decrement("CloudConnector_metric.decrement2", tags: new[] { "environment:dev" });
                dogStatsdService.Counter("CloudConnector_metric.count", 2, tags: new[] { "environment:dev" });
                dogStatsdService.Set("CloudConnector_metric.set", i, tags: new[] { "environment:dev" });
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
