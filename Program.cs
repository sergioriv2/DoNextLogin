using ServerlessLogin.Configs;
using ServerlessLogin.Seeders;

var builder = WebApplication.CreateBuilder(args);
var configurer = new AppConfiguration();
configurer.Configure(builder);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);


var app = builder.Build();

//if (args.Length == 1 && args[0].ToLower() == "seeddata")
//    SeedData(app);
//void SeedData(IHost app)
//{
//    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

//    using (var scope = scopedFactory.CreateScope())
//    {
//        var service = scope.ServiceProvider.GetService<Seed>();
//        service.SeedDataContext();
//    }
//}

app.UseCors(policyBuilder =>
    policyBuilder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .SetPreflightMaxAge(TimeSpan.FromMinutes(666))
                 );

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
