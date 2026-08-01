using HrPanel.Host;
using HrPanel.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHrPanelModules(builder.Configuration).AddUi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json","HR Panel API v1");
    });
}
else
{
    app.UseHsts();
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();

app.UseRequestLocalization();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
