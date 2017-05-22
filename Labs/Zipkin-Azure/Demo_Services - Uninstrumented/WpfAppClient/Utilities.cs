using System;
using Criteo.Profiling.Tracing;
using Criteo.Profiling.Tracing.Transport.Http;
using Criteo.Profiling.Tracing.Tracers.Zipkin;
using System.Threading;
using Criteo.Profiling.Tracing.Transport;

namespace WpfAppClient
{
    class Utilities
    {
        public enum ZKAnnotations { CS, SR, CR, SS };

        public static void RecordAnnotation(Trace trace, ZKAnnotations annotationType, String serviceName = null, string requestType = null)
        {
            switch (annotationType)
            {
                case ZKAnnotations.CS:
                    trace.Record(Annotations.ClientSend());
                    if (!String.IsNullOrEmpty(serviceName)) trace.Record(Annotations.ServiceName(serviceName));
                    if (!String.IsNullOrEmpty(requestType)) trace.Record(Annotations.Rpc("GET"));
                    break;
                case ZKAnnotations.CR:
                    trace.Record(Annotations.ClientRecv());
                    if (!String.IsNullOrEmpty(serviceName)) trace.Record(Annotations.ServiceName(serviceName));
                    if (!String.IsNullOrEmpty(requestType)) trace.Record(Annotations.Rpc("GET"));
                    break;
                case ZKAnnotations.SS:
                    trace.Record(Annotations.ServerSend());
                    if (!String.IsNullOrEmpty(serviceName)) trace.Record(Annotations.ServiceName(serviceName));
                    if (!String.IsNullOrEmpty(requestType)) trace.Record(Annotations.Rpc("GET"));
                    break;
                case ZKAnnotations.SR:
                    trace.Record(Annotations.ServerRecv());
                    if (!String.IsNullOrEmpty(serviceName)) trace.Record(Annotations.ServiceName(serviceName));
                    if (!String.IsNullOrEmpty(requestType)) trace.Record(Annotations.Rpc("GET"));
                    break;

            }

        }

        /// <summary>
        /// Initialize TraceManager with Tracer- holds Zipkin Server configuration like server URL, trace data format, Sampling rate.
        /// </summary>
        public static void InitializeTraceConfig(String zipkinServerUrl)
        {

            ILogger logger = new MyLogger(); 
            IZipkinSender zkSender = new HttpZipkinSender(zipkinServerUrl, "application/json");

            //1.0 implies full tracing without sampling ie., records all traces.
            TraceManager.SamplingRate = 1.0f;
            var tracer = new ZipkinTracer(zkSender, new JSONSpanSerializer());

            TraceManager.RegisterTracer(tracer);
            TraceManager.Start(logger);
        }

        public static void InjectTraceContextIntoHttpRequest(Trace currentTrace, System.Net.Http.HttpRequestMessage httpRequestMsg)
        {
            //Instantiate ZipkinTraceInjector which helps to transport trace context to WCF Service by injecting trace information into http Headers.
            ZipkinHttpTraceInjector zkTraceInjector = new ZipkinHttpTraceInjector();
            //Inject trace context into Http headers before calling WCF service.
            zkTraceInjector.Inject(currentTrace, httpRequestMsg.Headers, (c, key, value) => c.Add(key, value));
        }
    }
}
