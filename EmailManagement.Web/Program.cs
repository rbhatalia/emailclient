using Azure.Core;
using Email.IMAP;
using EmailClientCore;
using EmailManagement.Common;
using EmailManagement.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Graph;
using Nest;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddRazorPages();
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
    options.Scope.Add("offline_access");
    options.Scope.Add("IMAP.AccessAsUser.All");
    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var accessToken = context.AccessToken;
            // Save access token or user info as needed
        }
    };
});

builder.Services.AddScoped<IEmailAggregator, OutlookBasedService>(); // This injection can be abstracted to other providers like GMAIL etc.
builder.Services.AddHostedService<OutlookSyncService>();
builder.Services.AddTransient<IMessageSynchronizer, MessageSynchronizer>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();