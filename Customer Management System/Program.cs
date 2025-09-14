using System.Net;
using System.Net.Mail;
using Customer_Management_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            //Configures the connectionString for the database 
            var connectionString = builder.Configuration.GetConnectionString("DbConnection");

            //Configures the CustomerContext to the sql server database using the connectionString
            builder.Services.AddDbContext<CustomerContext>(options =>
                options.UseSqlServer(connectionString));

            // Configure FluentEmail with Gmail SMTP
            builder.Services
                .AddFluentEmail("mihlalikmlinganiso@gmail.com")
                .AddSmtpSender(new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("mihlalikmlinganiso@gmail.com", "fxxm dzyv qykw akug"),
                    EnableSsl = true
                });


            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

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

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Customer}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
