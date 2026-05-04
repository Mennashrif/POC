using Login.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapAccountEndpoints();
app.Run();
