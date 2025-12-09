using FCG.Notifications.Application.DependencyInjection;

namespace FCG.Notifications.Worker
{
    public class Program
    {
        protected Program() { }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                   .ConfigureServices((hostContext, services) =>
                   {
                       services.AddApplication(hostContext.Configuration);
                   });
        }
    }
}