using FormBuilder.Dtos;
using FormBuilder.Persiste;
using FormBuilder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Controllers;

// ---------- پنل ادمین ----------
[ApiController]
[Route("api/admin/pages")]
public class AdminPagesController : ControllerBase
{
    private readonly FormBuilderDbContext _db;
    private readonly PageService _service;

    public AdminPagesController(FormBuilderDbContext db, PageService service)
    {
        _db = db;
        _service = service;
    }

    // لیست همه صفحات
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var pages = await _db.FormPages
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(pages.Select(p =>
            new PageSummary(p.Id, p.Slug, p.Title, p.IsPublished, p.Components.Count)));
    }

    // ساخت صفحه جدید
    [HttpPost]
    public async Task<IActionResult> Create(CreatePageRequest req)
    {
        try
        {
            var page = await _service.CreateAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = page.Id }, page);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var page = await _db.FormPages.FindAsync(id);
        return page is null ? NotFound() : Ok(page);
    }

    // ذخیره عنوان و کامپوننت‌ها
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePageRequest req)
    {
        var page = await _db.FormPages.FindAsync(id);
        if (page is null) return NotFound();

        page.Title = req.Title;
        page.Components = req.Components;

        // مرتب‌سازی مجدد Order
        for (int i = 0; i < page.Components.Count; i++)
            page.Components[i].Order = i + 1;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var page = await _db.FormPages.FindAsync(id);
        if (page is null) return NotFound();

        var errors = page.ValidateAll();
        if (errors.Any()) return BadRequest(errors);

        page.Publish();
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        var page = await _db.FormPages.FindAsync(id);
        if (page is null) return NotFound();

        page.IsPublished = false;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id:guid}/clone")]
    public async Task<IActionResult> Clone(Guid id, CreatePageRequest req)
    {
        var clone = await _service.CloneAsync(id, req.Slug, req.Title);
        return Ok(clone);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var page = await _db.FormPages.FindAsync(id);
        if (page is null) return NotFound();

        _db.FormPages.Remove(page);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

// ---------- فرانت اپلیکیشن ----------
[ApiController]
[Route("api/pages")]
public class PublicPagesController : ControllerBase
{
    private readonly FormBuilderDbContext _db;
    public PublicPagesController(FormBuilderDbContext db) => _db = db;

    // مثال: GET /api/pages/home
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var page = await _db.FormPages
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (page is null) return NotFound();

        return Ok(new
        {
            page.Slug,
            page.Title,
            Components = page.Components
                .Where(c => c.IsVisible)
                .OrderBy(c => c.Order)
                .ToList()   // List<FormComponent> تا discriminator نوشته شود
        });
    }
}
