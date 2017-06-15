# Get diagnostics &amp; analytics for your ASP.NET Web App with Application Insights

# Overview

 Application Insights is a diagnostics and analytics solution for developers that provides fast, easy and non-intrusive analytics tools to interactively diagnose problems and answer tough questions. You can diagnose problems right from within your development environment and incorporate monitoring in your existing ALM workflows with Azure &amp; Visual Studio Team Services integrations. In this QuickStart, you'll learn how you can take advantage of these capabilities to identify, understand and solve problems in your ASP.NET Apps.

# Objectives

- Create an Azure Account for Application Insights (if do not have one)
- Add Application Insights to an ASP.NET App using Visual Studio 2017
- Diagnose issues in Application Insights Azure Portal
- Ask deeper questions with Analytics for Application Insights

# Prerequisites

- Windows 10
- Visual Studio 2017
- Microsoft Azure Subscription (sign up for free at [https://azure.microsoft.com/free/](https://azure.microsoft.com/free/))
  - If you already have an Azure Subscription, use the Microsoft Account associated with that.
  - If not and you have an MSDN subscription, you already have access to Microsoft Azure; you simply need to activate it in your MSDN benefits portal, if you have not already.
  - You can also [create a free trial Azure account](http://azure.microsoft.com/en-us/pricing/free-trial/), for which you would need a Credit Card for Security Verification

# Intended Audience

This Quick Start Challenge is intended for developers familiar with ASP.NET web apps and Visual Studio. It does not require prior experience with Application Insights (or diagnostics in general).

# Task 1: Create new ASP.NET app in Visual Studio with Application Insights

1. Open **Visual Studio 2017**.
2. Select **File | New | Project** from the main menu.
3. From the **Visual C# | Web** section, select the **ASP.NET Web Application (.NET Framework)** template and enter a **Name** of **"AppInsightsLab"**. Click **OK** to continue.

 ![](images/0.png)

1. Select the **MVC** template and click **OK** to create the project.

 ![](images/0.png)

1. Press **F5** to build and run the app. As you can see, there's nothing special here yet. Close the browser to end the debugging session. Note that you may need to select **Debug | Stop Debugging** to force an end to the session during this lab.
2. In **Solution Explorer** , right-click the **AppInsightsLab** project node and select **Application Insights | Configure Application Insights**.

 ![](images/0.png)

1. By default, the NuGet references to the core Application Insights packages are added. However, you'll need to connect with an Azure account to collect and analyze data. Click **Start Free**.

 ![](images/0.png)

1. Click **Sign in** and sign in with your Azure account.
2. Select a pricing mode and click **Register**.

 ![](images/0.png)

1. Click **Collect traces from System.Diagnostics** to add that trace listener to the project. This will enable you to send logs to Application Insights so that they can be merged in with other telemetry data collected in the application. Click **Finish** when the process completes.

 ![](images/0.png)

1. Let's walk through **Solution Explorer** to take a look at what was added and changed to the project. First, locate the **Application Insights** folder inside the **Connected Services** node. This provides a link to more information on getting started.

 ![](images/0.png)

1. Expand the **References** node and locate the **Microsoft.AI.** \* assemblies. These are the payloads of NuGet packages added by Application Insights.

 ![](images/0.png)

1. Double-click the **ErrorHandler\AiHandleErrorAttribute.cs** file to open it.

 ![](images/0.png)

1. This class contains a global exception handler that automatically sends unhandled exceptions to Application Insights for logging.

 ![](images/0.png)

1. Double-click the **Views\Shared\_Layout.cshtml** file to open it.

 ![](images/0.png)

1. This file is the outer-most template for HTML pages rendered by this application. Application Insights has inserted initialization code inside the tag so that you can immediately start using the JavaScript API to instrument the client-side portion of the application. Note the **instrumentationKey** property in the middle of the code block. This key is unique to your Application Insights account.

 ![](images/0.png)

1. Double-click **ApplicationInsights.config** to open it.

 ![](images/0.png)

 ![](images/0.png)

1. Finally, **packages.config** has been updated to reflect the NuGet packages and **Web.config** has been updated to reflect the assembly references.

 ![](images/0.png)

# Task 2: Tracking usage data

1. Press **F5** to build and run the project. Navigate around the site for a few pages using the navigation links at the top to generate traffic. Be sure to visit the **Contact** page.

 ![](images/0.png)

1. Leave the browser window open and return to **Visual Studio**.
2. Click the **Application Insights** button. Note that it may be minimized inside a collapsed menu button if the window is narrow.

 ![](images/0.png)

 ![](images/0.png)

1. The default action of the button is to open the **Application Insights Search**. This provides access to the full set of telemetry data collection throughout the application's history. Search for **"contact"** and click the first result that comes up.

 ![](images/0.png)

1. The **Request Details** that open provide a wealth of information about the collected data from the request, such as the request and response details. Click the **Track Operation** tab.

 ![](images/0.png)

1. The **Track Operation** tab provides the details and timeline of the steps involved in the request. In this case, it's a pretty simple single-step operation that succeeded.

 ![](images/0.png)

# Task 3: Working with the Application Insights Portal

1. Open a new browser window (not a new tab in the debug browser window) and navigate to [https://portal.azure.com](https://portal.azure.com/). The portal provides even more functionality.
2. Click **Application Insights** from the navigation menu.

 ![](images/0.png)

1. Click the **AppInsightsLab** account to open it.

 ![](images/0.png)

1. The **Overview** blade provides a dashboard view of the account. There are shortcut links and standard reports as well.

 ![](images/0.png)

1. Click the **Open chart in Analytics** button.

 ![](images/0.png)

1. This will open the extremely powerful **Analytics** engine for query and analysis. You can create virtually any kind of report you need using the flexible syntax and access to everything in the platform. Close the browser tab.

 ![](images/0.png)

1. Back on the **Overview** blade, click **Alerts**. Alerts are proactive notifications the platform can send you based on the circumstances you define.

 ![](images/0.png)

1. Click **Add alert**.

 ![](images/0.png)

1. Expand the **Metric** dropdown to see the list of metrics you can monitor for alerts. Close the alert blades until you return to the **Overview** blade.

 ![](images/0.png)

1. **Smart Detection** is a machine learning feature that learns about your app's behavior over time and can proactively identify circumstances that may be unexpected so that you can be alerted.

 ![](images/0.png)

1. Click **Availability**. Web tests are automated processes you can configure Application Insights to run periodically to ensure the application is running as expected.

 ![](images/0.png)

1. Click **Add web test**.

 ![](images/0.png)

1. You can create web tests that either ping a URL periodically or run a **.webtest** file you provide in order to determine whether the site is running as expected. Close the test blades until you return to the **Overview** blade for this project.

 ![](images/0.png)

1. Click the **Live Stream** button to begin live streaming the Application Insights collections into the portal.

 ![](images/0.png)

1. Return to the debug test app browser and use the navigation links to generate some site traffic. Close the debug application browser window (not the portal window) when done.

 ![](images/0.png)

1. Return to the portal window and notice how the traffic you just generated is already showing up.

 ![](images/0.png)

1. Close the **Live Metrics Stream** blade.
2. Click the **App map** button.

 ![](images/0.png)

 ![](images/0.png)

1. Besides the **Server** , the **Client** also has a dependency on **AJAX** requests, which you can see here.

 ![](images/0.png)

# Task 4: Tracking exceptions

1. Return the **Solution Explorer** in **Visual Studio** and double-click **Controllers\HomeController.cs** to open that file.

 ![](images/0.png)

1. Insert the following line of code at the top of the **Contact** method.

1.
  1. throw new Exception("Contact exception!");

1. The final method should look like this.

 ![](images/0.png)

1. Press **F5** to build and run the newly flawed project.
2. Navigate to the **Contact** page.

 ![](images/0.png)

1. This may throw an exception and break in **Visual Studio**. If so, press **F5** to continue anyway.
2. In **Visual Studio** , click the **Application Insights** button.

 ![](images/0.png)

1. There should be data in the view, but if there isn't, click the **Reset** button and set the **Time range** to **Last 30 minutes**. This occasionally needs to be done to refresh the view. Note that there is now an **Exception** tracked.

 ![](images/0.png)

1. Locate and click on the **Exception** in the list view.

 ![](images/0.png)

1. Like the request earlier, you can also click **Track Operation** to see the steps involved to get to this exception.

 ![](images/0.png)

1. As we already knew, the exception began with a request to **Home/Contact**.

 ![](images/0.png)

1. Expand the **Application Insights** dropdown and select **Explore Telemetry Trends**.

 ![](images/0.png)

1. The **Application Insights Trends** tool visualizes how your application's important telemetry events change over time, helping you quickly identify problems and anomalies. By linking you to more detailed diagnostic information, Trends can help you improve your app's performance, track down the causes of exceptions, and uncover insights from your custom events. Change the **Telemetry Type** to **Exceptions** and click **Analyze Telemetry** to see the trends for exceptions.

 ![](images/0.png)

1. Return to the portal window, which should still be open to the **Application Map**. Click **Refresh**.

 ![](images/0.png)

1. Note that the **Server** now indicates that there is a warning, which is a direct result of the exception. Click the warning. Note that it may take some time for the warning to appear.

 ![](images/0.png)

1. Scroll to the bottom of the new blade and click the exception.

 ![](images/0.png)

1. In the **Exceptions** panel, click **Contact exception!** to review its details.

 ![](images/0.png)

1. Even in the portal, you can review the full stack trace and other details about the exception.

 ![](images/0.png)

1. Return to **HomeController.cs** in **Visual Studio**. Note the two **CodeLens** markers on the right side of the **Contact** method. Click the last one to review **Exception** details. Click the one before it to view requests. These are pulling data from Application Insights to provide you with timely information in the editor.

 ![](images/0.png)

1. Close the debug app browser window.

# Task 5: Integrating telemetry

1. Add the following code inside the **About** method. This will track a server-side custom event every time someone visits **Home/About**.

1.
  1. var ai = new Microsoft.ApplicationInsights.TelemetryClient();
  2. TrackEvent("About - server");

1. In **Solution Explorer** , double-click **Views\Home\About.cshtml** to open it.

 ![](images/0.png)

1. Add the following code to the end of the file. It will use the JavaScript API to track a custom browser-side event when the **About** page is visited.

1.
  1. @section scripts
  2. {
  3. <script type="text/javascript">
  4. trackEvent("About - client");
  5. </script>
  6. }

1. Press **F5** to build and launch the site. Since **About.cshtml** was open, it should open to that page. Otherwise, navigate to it using the link at the top.
2. In **Visual Studio** , click **Application Insights**.

 ![](images/0.png)

1. Now you should see a **Custom Event** has been tracked, which is from the server-side telemetry.

 ![](images/0.png)

1. Return to the portal window and close all blades until you get back to the **AppInsightsLab** overview blade. Click the **Page view load time** chart.

 ![](images/0.png)

1. Click the **Page Views** chart.

 ![](images/0.png)

1. Click **Filters**.

 ![](images/0.png)

1. Check the **Custom Event** filter to add it. This will add browser-side events to the report. Click **Update**.

 ![](images/0.png)

1. On the **Search** blade, you will now see the custom event in the chart. You can now even drill in to the event to get more details, see other requests in the same session, and more.

 ![](images/0.png)

# Summary

Congratulations on completing this Quick Start Challenge! You are now ready to explore Application Insights on your own and discover how easy it is to set up alerts on your metrics, diagnose problems in client &amp; server side performance, identify dependency issues, set up web tests, filter &amp; segment data on out-of-the-box and custom properties, integrate with your existing DevOps processes, and so much more.

# Additional Resources

If you are interested in learning more about this topic, you can refer to the following resources:

**Documentation** : [https://www.visualstudio.com/en-us/docs/insights/application-insights](https://www.visualstudio.com/en-us/docs/insights/application-insights)

**GitHub** : [https://github.com/Microsoft/ApplicationInsights-Home](https://github.com/Microsoft/ApplicationInsights-Home)

**Team blog** : [https://azure.microsoft.com/en-us/blog/tag/application-insights/](https://azure.microsoft.com/en-us/blog/tag/application-insights/)
