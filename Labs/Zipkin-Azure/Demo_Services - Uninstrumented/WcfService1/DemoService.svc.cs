using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Azure;
using Criteo.Profiling.Tracing;
using Criteo.Profiling.Tracing.Transport.Http;
using Criteo.Profiling.Tracing.Tracers.Zipkin;
using Criteo.Profiling.Tracing.Transport;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DemoService : IDemoService
    {
        //You could replace this URL with your own Zipkin Server URL
        private const string zipkinServerUrl = "http://52.229.21.70:9411";
        public long SubmitNewJob(string content)
        {
            //Specify Zipkin server URL to transport traces to.

            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;

            //Extract trace information from Http headers, if not found create a new Trace without context.


            //Record sr (Server Receive) event to acknowledge receipt from WPF client.


            //app code
            Guid guidId = Guid.NewGuid();
            byte[] byteId = guidId.ToByteArray();
            long longId = BitConverter.ToInt64(byteId, 0);


            //Record ss (Server Send) event

            return (longId);
        }



    }

}
