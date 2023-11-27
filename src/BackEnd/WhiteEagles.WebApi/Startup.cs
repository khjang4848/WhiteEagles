namespace WhiteEagles.WebApi
{

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.HttpOverrides;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Autofac;

    using Infrastructure.Serialization;
    using Common;
    using Filters;
    using Data.Mapping;
    using Data.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(ConfigureJwtBearer);

            services.AddLogging();

            #region snippet_ConfigureServicesActionFilter
            services.AddScoped(container =>
            {
                var loggerFactory = container.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ClientIpCheckActionFilter>();

                return new ClientIpCheckActionFilter(
                    Configuration["AdminSafeList"], logger);
            });
            #endregion

            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            });

            services.ConfigureAutoMapper(typeof(Startup));

            services.AddSwaggerGen(ConfigureSwaggerGen);

            #region 추적 트랜젝션 테이블 등록 : YKLEE
            services.AddSingleton<WorkData.TxTable>();
            #endregion

            #region Background Service Regist : YKLEE
            services.AddHostedService<SubTasks.WithdrawCheckTask>();
            #endregion

            services.AddMvc(opt => opt.Filters.Add<GlobalExceptionFilter>())
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(new JsonWebToken(Configuration)).As<JsonWebToken>();
            builder.RegisterType<JsonTextSerializer>().As<ITextSerializer>();
            builder.RegisterType<AccountInquiryService>().As<IAccountInquiryService>();
            builder.RegisterType<MerchantInfoService>().As<IMerchantInfoService>();
            builder.RegisterType<TransferService>().As<ITransferService>();
            builder.RegisterType<WalletService>().As<IWalletService>();
            builder.RegisterType<CalculateService>().As<ICalculateService>();
            builder.RegisterType<PGInquiryService>().As<IPGInquiryService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhiteEagles.WebApi v1"));

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhiteEagles.WebApi v1"));
            //}

            app.UseRouting();

            app.UseAuthorization();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

        private static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WhiteEagles.WebApi",
                Version = "v1"
            });
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "bearer"
            });
            options.OperationFilter<AuthenticationRequirementsOperationFilter>();
        }

        private void ConfigureJwtBearer(JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
