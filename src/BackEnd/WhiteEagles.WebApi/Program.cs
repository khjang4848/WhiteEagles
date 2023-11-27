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

            #region 출금 추적 테이블 복구 로직 처리 : YKLEE
            
            // 출금 트랜젝션 로그 DB 에서 상태값이 대기중인 트랜젝션을 가져다 등록 해준다.
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
