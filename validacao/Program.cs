using validacao.Services.Abstractions;
using validacao.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Registrar serviços com injeção de dependência
builder.Services.AddScoped<ILuhnValidator, LuhnValidator>();
builder.Services.AddScoped<ICardBrandService, CardBrandService>();
builder.Services.AddScoped<ICreditCardValidationService, CreditCardValidationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowReact",
      policy => policy
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowReact");

app.MapControllers();

app.Run();
