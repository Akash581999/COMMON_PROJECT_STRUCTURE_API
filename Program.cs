using System.Text.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using COMMON_PROJECT_STRUCTURE_API.services;

WebHost.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        services.AddSingleton<login>();
        services.AddSingleton<register>();
        services.AddSingleton<editProfile>();
        services.AddSingleton<deleteProfile>();

        services.AddAuthorization();
        services.AddControllers();
        services.AddCors();
        services.AddAuthentication("SourceJWT").AddScheme<SourceJwtAuthenticationSchemeOptions, SourceJwtAuthenticationHandler>("SourceJWT", options =>
        {
            options.SecretKey = appsettings["jwt_config:Key"].ToString();
            options.ValidIssuer = appsettings["jwt_config:Issuer"].ToString();
            options.ValidAudience = appsettings["jwt_config:Audience"].ToString();
            options.Subject = appsettings["jwt_config:Subject"].ToString();
        });
    })
    .Configure(app =>
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors(options =>
            options.WithOrigins("https://localhost:5002", "http://localhost:5001")
            .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
        app.UseRouting();
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            var login = endpoints.ServiceProvider.GetRequiredService<login>();
            var register = endpoints.ServiceProvider.GetRequiredService<register>();
            var editProfile = endpoints.ServiceProvider.GetRequiredService<editProfile>();
            var deleteProfile = endpoints.ServiceProvider.GetRequiredService<deleteProfile>();

            endpoints.MapGet("/login",
           [AllowAnonymous] async (HttpContext http) =>
           {
               var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
               requestData rData = JsonSerializer.Deserialize<requestData>(body);
               if (rData.eventID == "1001") // update
                   await http.Response.WriteAsJsonAsync(await login.Login(rData));

           });

            endpoints.MapPost("register",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1002") // update
                    await http.Response.WriteAsJsonAsync(await register.Register(rData));

            });
            endpoints.MapPut("editProfile",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1003") // update
                    await http.Response.WriteAsJsonAsync(await editProfile.EditProfile(rData));

            });

            endpoints.MapDelete("deleteProfile",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1004") // update
                    await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfile(rData));

            });

            endpoints.MapGet("/bing",
                 async c => await c.Response.WriteAsJsonAsync("{'Name':'Akash','Age':'24','Project':'COMMON_PROJECT_STRUCTURE_API'}"));
        });
    }).Build().Run();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

public record requestData
{
    [Required]
    public string eventID { get; set; }
    [Required]
    public IDictionary<string, object> addInfo { get; set; }
}

public record responseData
{
    public responseData()
    {
        eventID = "";
        rStatus = 0;
        rData = new Dictionary<string, object>();
    }
    [Required]
    public int rStatus { get; set; } = 0;
    public string eventID { get; set; }
    public IDictionary<string, object> addInfo { get; set; }
    public IDictionary<string, object> rData { get; set; }
}
