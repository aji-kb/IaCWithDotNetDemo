using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Rest;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace IaCWithDotNetDemo
{
    public class DeployManager
    {
        public void SampleDeploy()
        {
            string tenantID = ConfigurationManager.AppSettings["tenantID"];
            string clientId = ConfigurationManager.AppSettings["clientId"]; ;
            string clientSecret = ConfigurationManager.AppSettings["clientSecret"];
            string subscriptionId = ConfigurationManager.AppSettings["AzureSubscriptionId"];

            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantID, AzureEnvironment.AzureGlobalCloud);
            var azure = Azure.Configure().WithLogLevel(Microsoft.Azure.Management.ResourceManager.Fluent.Core.HttpLoggingDelegatingHandler.Level.Basic).Authenticate(credentials).WithSubscription(subscriptionId);

            var templateString = File.ReadAllText("storageaccountarmtemplate.json");

            JObject template = JObject.Parse(templateString);
            template.SelectToken("parameters.storageAccounts_samplestorage2020_name")["defaultValue"] = "demostorage" + new Random().Next(101, 199);//Generate Unique name for the storage account
            var resourceName = "storage" + Guid.NewGuid().ToString();
            var resourceGroupName = "rglearnazure";
            azure.Deployments.Define(resourceName).WithExistingResourceGroup(resourceGroupName).WithTemplate(template.ToString()).WithParameters("{}").WithMode(DeploymentMode.Incremental).Create();

        }
    }
}
