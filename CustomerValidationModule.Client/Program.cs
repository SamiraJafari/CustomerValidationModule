using CustomerValidationModule.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<ICustomerValidationService, MockCustomerValidationService>();

await builder.Build().RunAsync();
