using LkDataConnection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Task_Management.Classes;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };


    });





builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterConnection", policy =>
    {
        policy.WithOrigins("*")
        .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ConnectionClass>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedForHeaderName = "X-Coming-From";
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseCors("FlutterConnection");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Connection.ConnectionStr = new
    ConnectionClass(builder.Configuration).GetSqlConnection().ConnectionString;
Connection.Connect();
//builder.Configuration["ConnectionStrings:Issuer"]

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseForwardedHeaders();

app.UseAuthorization();
app.MapControllers();

app.Run();
