using identity.user;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectIdentity(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.InjectMySwagger();

var app = builder.Build();

Console.WriteLine("Setting environment variables for each target..., how production\n ");

if (app.Environment.IsDevelopment())
{
    app.UseMyDocumentation();
    Console.WriteLine("Setting environment variables for each target..., how development\n ");
    Environment.SetEnvironmentVariable("UrlBase", "http://localhost:5000");
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
