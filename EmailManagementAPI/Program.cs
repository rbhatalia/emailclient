using EmailManagement.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;

var builder = WebApplication.CreateBuilder(args);
// Access Configuration
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ElasticsearchService>(provider =>
{
    return new ElasticsearchService(configuration["Elasticsearch:Uri"]);
});
builder.Services.AddSingleton<UserDatabase>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Microsoft";
})
.AddCookie()
.AddMicrosoftAccount("Microsoft", options =>
{
    options.ClientId = configuration["Authentication:Microsoft:ClientId"];
    options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
    options.CallbackPath = "/signin-microsoft";
    options.SaveTokens = true;
    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var accessToken = context.AccessToken;
            // Save access token or user info as needed
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app!.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
