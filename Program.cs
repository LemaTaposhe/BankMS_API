using BankMS_API.Data;
using BankMS_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the database context to use SQL Server
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbCon")));

// JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Authorization Policies configuration
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("BankEmployee"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// Optionally, add a Hosted Service (e.g., for interest calculation)
builder.Services.AddHostedService<InterestCalculationService>();
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Seed the database with initial data if it's the first time the application runs
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BankDbContext>();

    // Call the method to seed data
    BankDbContext.InitializeSeedData(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger for API documentation
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage(); // For development environment
app.UseHttpsRedirection();
// Apply CORS policy
app.UseCors("AllowOrigin");
// Add middleware for JWT authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers for API routing
app.MapControllers();

// Run the application
app.Run();

