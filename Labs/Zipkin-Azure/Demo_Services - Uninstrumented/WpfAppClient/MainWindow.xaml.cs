using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Criteo.Profiling.Tracing;
using Criteo.Profiling.Tracing.Transport.Http;
using Criteo.Profiling.Tracing.Tracers.Zipkin;
using System.Threading;
using Criteo.Profiling.Tracing.Transport;
using Criteo.Profiling.Tracing.Tracers;

namespace WpfAppClient
{
    
    public partial class MainWindow : Window
    {
        //You could replace this URL with your own Zipkin Server URL
        private const string zipkinServerUrl = "http://52.229.21.70:9411";
        // Counts the number of requests submitted.
        int count = 1;
        public MainWindow()
        {
            InitializeComponent();
            //Specify Zipkin server URL to transport traces to.
                     
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Create new Trace.
          

            //Record sr(server receive) and ss(server send) events to acknowledge click event.
           
          
            //Create child trace and record (cs) Client Send event before starting a new Span.
          

            //Update number of requests so far on click button.
            var title = string.Format("{0} clicks!", count++);
            Button.Content = title;

            //Call WCF service with input text
            Task<string> response = SubmitJob(inputBox.Text);
          
            //Assign trace id infomation to text boxes. 
            string text = await response;
            long output = long.Parse(text);
            System.Diagnostics.Debug.WriteLine("Ouput:" + string.Format("{0:x}", output));
            TextBox.Text = string.Format("{0:x}", output);
            aiId.Text = output.ToString();

            //Record cr (child receive) event to complete current Span.

            
        }

        /// <summary>
        /// Method to invoke WCF Service SubmitNewJob Operation with the input content from User.
        /// </summary>
        /// <param name="content">user input</param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private async Task<string> SubmitJob(string content)
        {            
                       
            //Initialze WCF Service URL and HttpClient used to call the Service.
            Uri geturi = new Uri(string.Format("http://localhost:1331/DemoService.svc/SubmitNewJob/?content={0}", content));
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, geturi);
           
            //Inject trace context into Http headers before calling WCF service.
                       
            //Call WCF service.
            System.Net.Http.HttpResponseMessage responseGet = await client.SendAsync(requestMsg);

            //Read WCF Service response 
            string response = await responseGet.Content.ReadAsStringAsync();
            int index = response.IndexOf("</long>");
            if (index > 0)
            {
                int start = response.Substring(0, index).LastIndexOf('>');
                return response.Substring(start + 1, index - start - 1);
            }
            return response;
        }
    }
}
