using System;
using System.IO;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Services.Core;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;

namespace DigitalInspectionNetCore21
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
	        services.AddCors();

			ConfigureMySql(services);

			services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

	        ConfigureSwaggerGenerator(services);

			services.AddMvc()
	            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
		        .AddJsonOptions(
		            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
		        );

			RegisterDependencies(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCookiePolicy();

	        ConfigureStaticFileHandling(app);
			ConfigureSwaggerUI(app);

			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

		#region IServicesCollection Helpers

	    private void RegisterDependencies(IServiceCollection services)
	    {
		    services.AddScoped<ITagRepository, TagRepository>();
		    services.AddScoped<IChecklistItemRepository, ChecklistItemRepository>();
		}

		private void ConfigureMySql(IServiceCollection services)
	    {
		    //services.AddDbContext<ApplicationDbContext>(options =>
		    //	options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

		    services.AddDbContextPool<ApplicationDbContext>(
			    options => options.UseMySql(Configuration["ConnectionStrings:DefaultConnection"],

				    mysqlOptions =>
				    {
					    mysqlOptions.ServerVersion(new Version(5, 7, 17), ServerType.MySql); // replace with your Server Version and Type
				    }
			    ));
		}

	    private void ConfigureSwaggerGenerator(IServiceCollection services)
	    {
		    // Register the Swagger generator, defining 1 or more Swagger documents
		    services.AddSwaggerGen(c =>
		    {
			    c.SwaggerDoc("v1.3", new Info
			    {
				    Title = "Digital Inspection API",
				    Version = "v1.3",
				    Description = "An API used for capturing digital vehicle inspections for Murphy Automotive",
				    TermsOfService = "Contact before using!",
				    Contact = new Contact
				    {
					    Name = "Daniel Caspers",
					    Email = "daniel@caspersonline.us",
					    Url = "https://linkedin.com/in/danielcaspers"
				    }
			    });
		    });
		}

		#endregion

		#region IApplicationBuilder Helpers

		private void ConfigureStaticFileHandling(IApplicationBuilder app)
	    {
		    // For serving files out of /wwwroot/
		    app.UseStaticFiles();

		    // For serving inspection images out of /Uploads/
		    app.UseStaticFiles(new StaticFileOptions
		    {
				// TODO DJC Force ASP.NET Core to create folder if it doesn't exist
			    FileProvider = new PhysicalFileProvider(
				    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
			    RequestPath = "/Uploads"
		    });
		}



	    private void ConfigureSwaggerUI(IApplicationBuilder app)
	    {
		    // Enable middleware to serve generated Swagger as a JSON endpoint.
		    app.UseSwagger();

		    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
		    // specifying the Swagger JSON endpoint.
		    app.UseSwaggerUI(c =>
		    {
			    c.SwaggerEndpoint("/swagger/v1.3/swagger.json", "Digital Inspection API V1.3");
			    c.RoutePrefix = string.Empty;
			    c.InjectStylesheet("/swagger/ui/theme-material.css");
		    });
		}

		#endregion
	}
}
