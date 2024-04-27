namespace BackgroundWorker.Api.Extensions;

public static class AppExtensions
{
    public static WebApplication UseArchitectures(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}
