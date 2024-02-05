using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(exception =>
{
    exception.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = Text.Plain;

        var error = context
        .Features
        .GetRequiredFeature<IExceptionHandlerFeature>()
        .Error;

        await context.Response.WriteAsJsonAsync(
            new ErrorDto
            {
                ErrorMessage = error.Message,
                Data = error.Data
            });
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();