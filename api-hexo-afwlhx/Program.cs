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

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

// 自动寻找index.html
app.UseDefaultFiles();

// 使用静态资源
app.UseStaticFiles();

// // 处理所有其他请求，返回404.html
// app.MapFallbackToFile("/404.html");

// 3. 对未知路径做 404 跳转逻辑（放在最后）
app.Use(async (context, next) =>
{
    await next();

    // 如果返回404，并且不是 /api 开头的请求
    if (context.Response.StatusCode == 404 && 
        !context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.StatusCode = 200; // 改成200，否则有些浏览器不加载
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "404.html"));
    }
});

app.Run();