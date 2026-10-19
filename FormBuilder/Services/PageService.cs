using FormBuilder.Components;
using FormBuilder.Dtos;
using FormBuilder.Pages;
using FormBuilder.Persiste;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FormBuilder.Services;

public class PageService
{
    private readonly FormBuilderDbContext _db;
    public PageService(FormBuilderDbContext db) => _db = db;

    public async Task<FormPage> CreateAsync(CreatePageRequest req)
    {
        var slug = req.Slug.Trim().ToLowerInvariant();

        if (await _db.FormPages.AnyAsync(p => p.Slug == slug))
            throw new InvalidOperationException("این slug قبلاً استفاده شده است.");

        var page = new FormPage { Slug = slug, Title = req.Title };
        _db.FormPages.Add(page);
        await _db.SaveChangesAsync();
        return page;
    }

    public async Task<FormPage> CloneAsync(Guid sourceId, string newSlug, string newTitle)
    {
        var source = await _db.FormPages.AsNoTracking().FirstAsync(p => p.Id == sourceId);

        // Deep copy از طریق serialize/deserialize
        var json = JsonSerializer.Serialize(source.Components);
        var components = JsonSerializer.Deserialize<List<FormComponent>>(json)!;

        // شناسه جدید برای هر کامپوننت
        foreach (var c in components)
            c.Id = Guid.NewGuid();

        var clone = new FormPage
        {
            Slug = newSlug.Trim().ToLowerInvariant(),
            Title = newTitle,
            Components = components
        };

        _db.FormPages.Add(clone);
        await _db.SaveChangesAsync();
        return clone;
    }
}