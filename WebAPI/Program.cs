using Microsoft.EntityFrameworkCore;
using WebAPI.CustomActionFilter;
using WebAPI.Data;
using WebAPI.Repositories;
using WebAPI.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// register DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IBookRepository, SQLBookRepository>();
builder.Services.AddScoped<IAuthorRepository, SQLAuthorRepository>();
builder.Services.AddScoped<IPublisherRepository, SQLPublisherRepository>();
builder.Services.AddScoped<IBook_AuthorRepository, SQLBook_Author>();
builder.Services.AddScoped<ValidatePublisherExistsAttribute>();
builder.Services.AddScoped<ValidateBookAuthorNotExistsAttribute>();
builder.Services.AddScoped<ValidateAuthorCanDeleteAttribute>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ValidateBookJsonFieldsMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
