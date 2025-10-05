using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace api_hexo_afwlhx.Controllers;

[ApiController]
[Route("api/[action]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, string key)
    {
        if (key != "248655") return BadRequest(new { message = "错误key!" });

        if (file == null || file.Length == 0)
            return BadRequest("没有上传文件");

        // 检查是否是 zip
        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            return BadRequest("请上传 zip 文件");

        // 临时路径
        var tempPath = Path.Combine(_env.ContentRootPath, "temp");
        Directory.CreateDirectory(tempPath);

        var zipPath = Path.Combine(tempPath, file.FileName);

        // 保存 zip
        using (var stream = new FileStream(zipPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 解压路径（wwwroot）
        var wwwrootPath = Path.Combine(_env.ContentRootPath, "wwwroot");

        // 清空旧文件（可选，视需求）
        foreach (var dir in Directory.GetDirectories(wwwrootPath))
            Directory.Delete(dir, true);
        foreach (var f in Directory.GetFiles(wwwrootPath))
            System.IO.File.Delete(f);

        // 解压
        ZipFile.ExtractToDirectory(zipPath, wwwrootPath);

        // 删除 zip
        System.IO.File.Delete(zipPath);

        return Ok(new { message = "网站已更新成功" });
    }
}