


using Autofac;
using Autofac.Extensions.DependencyInjection;
using Identity.Api.Messaging;
using Identity.Api.Models;
using Identity.Api.Services;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace Identity.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IContainer ApplicationContainer { get; private set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton(sp =>
            {
                var configuration = new ConfigurationOptions { ResolveDns = true };
                configuration.EndPoints.Add(Configuration["RedisHost"]);
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddTransient<IIdentityRepository, IdentityRepository>();
            var builder = new ContainerBuilder();

            // register a specific consumer
            builder.RegisterType<ApplicantAppliedEventConsumer>();

            builder.Register(context =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://rabbitmq/"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(host, "AppContainer" + Guid.NewGuid().ToString(), e =>
                    {
                        //e.LoadFrom(context);
                        e.Consumer<ApplicantAppliedEventConsumer>();
                    });
                });

                return busControl;
            })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (lifetime == null)
            {
                throw new ArgumentNullException(nameof(lifetime));
            }

            app.UseHttpsRedirection();
            app.UseMvc();


            var identityRepository = serviceProvider.GetService<IIdentityRepository>();
            await identityRepository.UpdateUserAsync(new User { Id = "1", Email = "josh903902@gmail.com", Name = "Josh Dillinger" });

            var bus = ApplicationContainer.Resolve<IBusControl>();
            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            lifetime.ApplicationStopping.Register(() => busHandle.Stop());
        }
    }
}
