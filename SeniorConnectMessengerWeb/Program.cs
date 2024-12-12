using Infrastructure.DataAccessLayer;
using SeniorConnectMessengerWeb.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using core.Helpers;
using Core.Services;

namespace SeniorConnectMessengerWeb
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<IChatStorage>(repo => new ChatRepository(builder.Configuration.GetConnectionString("SeniorConnectContext")));
			builder.Services.AddScoped<IUserStorage>(repo => new UserRepository(builder.Configuration.GetConnectionString("SeniorConnectContext")));
			builder.Services.AddScoped<UserService>();
			builder.Services.AddScoped<ChatService>();
			builder.Services.AddScoped<ChatUpdateService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options => 
				{
					options.LoginPath = new PathString("/Auth/Login");
				});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseAuthentication();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
