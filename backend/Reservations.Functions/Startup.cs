﻿using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Functions;
using Scrutor;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Reservations.Functions
{
    public class Startup : FunctionsStartup
    {
        //public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        //{
        //    builder.ConfigurationBuilder
        //        .AddEnvironmentVariables()
        //        .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        //}

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            var services = builder.Services;
            services.AddAzureClients(x =>
            {
                x.UseCredential(CreateTokenCredential());

                var azureWebJobsStorage = configuration.GetValue<string>("AzureWebJobsStorage");
                x.AddTableServiceClient(azureWebJobsStorage);
                x.AddQueueServiceClient(azureWebJobsStorage);
            });

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Startup))
                .AddClasses()
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithTransientLifetime());
        }

        private static TokenCredential CreateTokenCredential()
        {
            return new ChainedTokenCredential(
#if DEBUG
                new AzureCliCredential(),
#endif
                new ManagedIdentityCredential());
        }
    }
}