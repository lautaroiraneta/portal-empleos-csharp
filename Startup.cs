using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace PortalEmpleos
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
			services.AddCors(c =>
			{
				c.AddPolicy("AllowOrigin", options => options.WithOrigins("https://localhost:4200"));
			});

			services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
				options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
				options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			});

			services.AddSwaggerGen(config =>
			{
				config.SwaggerDoc("v1", new OpenApiInfo()
				{
					Title = "Portal Empleos API"
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				app.UseSwagger();
				app.UseSwaggerUI(config =>
				{
					config.SwaggerEndpoint("/swagger/v1/swagger.json", "Portal Empleos API");
				});

				app.UseCors(
					options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader()
				);
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
