using identity.user;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectIdentity(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.InjectMySwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMyDocumentation();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
