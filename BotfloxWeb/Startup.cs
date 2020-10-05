using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Botflox.Bot;
using Botflox.Bot.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XivApi;
using XivApi.Character;

namespace BotfloxWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddDiscord(options => {
                    options.ClientId = Configuration["Authentication:Discord:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Discord:ClientSecret"];
                });
            services.AddRazorPages(options => {
                // Razor options
            });
            services.AddAuthorization(options => {
                // Policies here
            });
            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            BotfloxInit.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapGet("/discord", async context => {
                    context.Response.Redirect("https://discord.gg/NvQ5Udx");
                    await context.Response.CompleteAsync();
                });
                endpoints.MapGet("/invite", async context => {
                    BotfloxBot bot = context.RequestServices.GetRequiredService<BotfloxBot>();
                    context.Response.Redirect((await bot.GetInviteUriAsync()).ToString());
                    await context.Response.CompleteAsync();
                });
                endpoints.MapGet("/profile/by-lodestone/{id}", async context => {
                    IServiceProvider sp = context.RequestServices;
                    HttpResponse response = context.Response;
                    if (!ulong.TryParse(context.Request.RouteValues["id"] as string, out ulong lodestoneId)) {
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        await response.CompleteAsync();
                        return;
                    }

                    XivApiClient xivApiClient = sp.GetRequiredService<XivApiClient>();
                    CharacterProfileGeneratorService profileGeneratorService =
                        sp.GetRequiredService<CharacterProfileGeneratorService>();

                    CharacterProfile profile =
                        await xivApiClient.CharacterProfileAsync(lodestoneId, context.RequestAborted);
                    Image profileImage = await profileGeneratorService.RenderCharacterProfileAsync(profile);
                    response.ContentType = "image/png";
                    await Task.Run(() => profileImage.Save(response.Body, ImageFormat.Png), context.RequestAborted);
                });
                endpoints.MapRazorPages();
            });
        }
    }
}