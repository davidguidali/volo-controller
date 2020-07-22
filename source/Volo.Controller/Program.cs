using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Volo.Controller.Shared;

namespace Volo.Controller.Opcua
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    aliases: new[] { "--settings" },
                    description: "The path to the settings file")
            };

            rootCommand.Description = "Creates the OPCUA controller" +
                " for the volo project";
            rootCommand.Handler = CommandHandler.Create<string>(Initialize);
            await rootCommand.InvokeAsync(args);
        }

        private static async Task Initialize(string settings)
        {
            var appSettings = GetAppSettings(settings);
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(appSettings);
            serviceCollection.AddSingleton<OpcuaController>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var opcuaController = serviceProvider.GetRequiredService<OpcuaController>();

            await opcuaController.Start();
        }

        private static AppSettings GetAppSettings(string settings)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(settings);
            var config = configBuilder.Build();

            var appSettings = new AppSettings();
            config.Bind("applicationConfig", appSettings);

            return appSettings;
        }
    }
}