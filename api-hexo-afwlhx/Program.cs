var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 添加 CORS 服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// 自动寻找index.html
app.UseDefaultFiles();

// 使用静态资源
app.UseStaticFiles();

app.MapControllers();

// 注意：不要用 MapFallbackToFile！
// 改用一个自定义的中间件来判断是否存在文件
app.Use(async (context, next) =>
{
    await next(); // 让请求先经过前面的管道（包括静态文件、API）

    // 如果满足以下条件，则返回 404.html
    if (context.Response.StatusCode == 404 && // 状态码是404
        !context.Request.Path.StartsWithSegments("/api") && // 不带 /api 路径
        !Path.HasExtension(context.Request.Path.Value)) // 不含扩展名
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "404.html"));
    }
});

app.Run();