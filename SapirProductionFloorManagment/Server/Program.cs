using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.IdentityModel.Tokens;
using SapirProductionFloorManagment.Client.Logic;
using SapirProductionFloorManagment.Server;
using SapirProductionFloorManagment.Server.Authentication;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddBlazoredToast();


builder.Services.AddAuthentication(O =>
{
    O.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;   
    O.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAuthenticationManager.JWT_SECURITY_KEY)),
        ValidateIssuer = false,
        ValidateAudience = false,   
    };

});
 

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())

{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();    
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
