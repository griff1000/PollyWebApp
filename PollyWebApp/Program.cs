namespace PollyWebApp
{
    using Controllers;
    using global::Polly;
    using global::Polly.Contrib.WaitAndRetry;
    using Polly;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Adding the HTTP client

            // No policies applied globally; any policies applied per operation in WeatherController
            builder.Services.AddHttpClient();

            // All policies applied globally here - this is the recommended way of doing it FOR HTTP(S) CALLS
            //builder.Services
            //    .AddHttpClient(nameof(WeatherController)) // Must be a named client
            //    .AddTransientHttpErrorPolicy(pb =>
            //        pb.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

            // All policies applied globally here - this is to demo how you could use it for non-HTTP(S) calls
            //builder.Services
            //    .AddHttpClient(nameof(WeatherController)) // Must be a named client
            //    .AddPolicyHandler(RetryPolicies.JitteredExponentialBackoffRetryPolicy);

            #endregion


            builder.Services.AddControllersWithViews();

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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}