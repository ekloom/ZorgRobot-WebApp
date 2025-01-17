using System.Reflection;
using ZorgRobotWebApp.Components;
using ZorgRobotWebApp.Services;
using ZorgRobotWebApp.Services.AgendaManager;
using ZorgRobotWebApp.Services.Datainterface;
using ZorgRobotWebApp.Services.Mqtt;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetEntryAssembly()!);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(o => SimpleMqttClient.CreateSimpleMqttClientForHiveMQ("ZorgRobotBlazorApp"));

builder.Services.AddHostedService<MqttMessageHandler>();

builder.Services.AddSingleton<SqlInterface>();

builder.Services.AddSingleton<TaskUtil>();

builder.Services.AddSingleton<SqlTaskRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
