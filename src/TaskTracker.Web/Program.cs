using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskTracker.Web;
using TaskTracker.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]!;

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBaseUrl))
    .AddHttpMessageHandler<CookieHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<BoardService>();
builder.Services.AddScoped<TaskItemService>();
await builder.Build().RunAsync();
