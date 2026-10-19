using FormBuilder.Components;
using FormBuilder.Pages;
using FormBuilder.Persiste;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Controllers;

[ApiController]
[Route("api/pages")]
public class PagesController : ControllerBase
{
    private readonly FormBuilderDbContext _db;
    public PagesController(FormBuilderDbContext db) => _db = db;

    // لیست کامپوننت‌های موجود برای پنل ادمین
    [HttpGet("component-types")]
    public IActionResult GetTypes() => Ok(ComponentRegistry.All);

    // افزودن کامپوننت خالی به صفحه
    [HttpPost("{pageId}/components/{type}")]
    public async Task<IActionResult> AddComponent(Guid pageId, string type)
    {
        var page = await _db.FormPages.FindAsync(pageId);
        if (page is null) return NotFound();

        var component = ComponentRegistry.CreateEmpty(type);
        component.Order = page.Components.Count + 1;
        page.Components.Add(component);

        await _db.SaveChangesAsync();
        return Ok(component);
    }

    // ذخیره کل صفحه با مقادیر پر شده
    [HttpPut("{pageId}")]
    public async Task<IActionResult> Update(Guid pageId, FormPage input)
    {
        var page = await _db.FormPages.FindAsync(pageId);
        if (page is null) return NotFound();

        page.Title = input.Title;
        page.Components = input.Components;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{pageId}/publish")]
    public async Task<IActionResult> Publish(Guid pageId)
    {
        var page = await _db.FormPages.FindAsync(pageId);
        if (page is null) return NotFound();

        var errors = page.ValidateAll();
        if (errors.Any()) return BadRequest(errors);

        page.Publish();
        await _db.SaveChangesAsync();
        return Ok();
    }

    // خروجی برای فرانت اپلیکیشن
    [HttpGet("{pageId}")]
    public async Task<IActionResult> Get(Guid pageId)
    {
        var page = await _db.FormPages.FindAsync(pageId);
        if (page is null || !page.IsPublished) return NotFound();

        return Ok(new
        {
            page.Id,
            page.Title,
            Components = page.Components
                .Where(c => c.IsVisible)
                .OrderBy(c => c.Order)
        });
    }
}