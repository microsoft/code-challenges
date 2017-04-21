Azure Search
======

# Deployment

Deploy a free azure search instance using arm template below.

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2Fcode-challenges%2Fmaster%2FLabs%2FAzure%2520Search%2Fazure-search-deploy.json)
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2Fcode-challenges%2Fmaster%2FLabs%2FAzure%2520Search%2Fazure-search-deploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>

Once the Azure Search instance is deployed use console application in the utilities folder `"Microsoft.CodeChallenges.AzureSearch.DataSeed"` to seed the search with data. The console application will request the name of your Azure Search instance and the API key. these can both be found in the output from the ARM deployment.

Before Running the lab open the lab application `"Microsoft.CodeChallenges.AzureSearch.Lab"` and open the file `"Configuration\SearchConfig.cs"` and update the configuration with the name of your Azure Search instance and the API key.

# Lab
See the hands-on lab [here](hands-on-lab.md)
