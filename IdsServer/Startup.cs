using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdsServer.Ids;
using IdsServer.ValidateExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdsServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddIdentityServer(options =>
            {
                //����ͨ����������ָ����¼·����Ĭ�ϵĵ�½·����/account/login
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
              //���֤����ܷ�ʽ��ִ�и÷����������ж�tempkey.rsa֤���ļ��Ƿ���ڣ���������ڵĻ����ʹ���һ���µ�tempkey.rsa֤���ļ���������ڵĻ�����ʹ�ô�֤���ļ���
              .AddDeveloperSigningCredential()
              //���ܱ�����Api��Դ��ӵ��ڴ���
              .AddInMemoryApiResources(IdsConfig.GetApiResources())
              //�ͻ���������ӵ��ڴ���
              //.AddInMemoryClients(OAuthMemoruData.GetClients())
              //���Ե��û���ӽ���
              //.AddTestUsers(OAuthMemoruData.GetTestUsers())
              //     .AddInMemoryIdentityResources(new List<IdentityResource>
              //{
              //    new IdentityResources.OpenId(), //δ��ӵ���scope����
              //    new IdentityResources.Profile()
              //})
              //�����Ϣ��Դ
              .AddInMemoryIdentityResources(IdsConfig.GetIdentityResources())
              //����Զ���ͻ���
              .AddClientStore<ClientStore>()
              //����Զ����˺�����ķ���
              .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
              //����΢����֤��½�ķ���
              .AddExtensionGrantValidator<WechatLoginValidator>()
              .AddInMemoryApiScopes(IdsConfig.ApiScopes);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseIdentityServer();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
