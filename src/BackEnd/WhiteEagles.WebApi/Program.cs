namespace WhiteEagles.WebApi
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            #region ��� ���� ���̺� ���� ���� ó�� : YKLEE
            
            // ��� Ʈ������ �α� DB ���� ���°��� ������� Ʈ�������� ������ ��� ���ش�.
            #endregion

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddFile("Logs/myapp-{Date}.txt");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:9988", "http://*:9989");
                    webBuilder.UseStartup<Startup>();

                });
    }
}
