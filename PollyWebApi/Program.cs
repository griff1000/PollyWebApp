namespace PollyWebApi
{
    using Middleware;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            #region  You can add transient faults by enabling one or the other of these middleware components

            app.UseFakeTransientErrorMiddleware();
            //app.UseFakeTransientSlowResponseMiddleware();

            #endregion

            app.MapControllers();

            app.Run();
        }
    }
}