using FormBuilder.Components;

namespace FormBuilder.Pages;

public class FormPage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = default!;
    public string Title { get; set; } = default!;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public List<FormComponent> Components { get; set; } = new();

    public IReadOnlyList<string> ValidateAll()
        => Components.SelectMany(c => c.Validate()).ToList();

    public void Publish()
    {
        var errors = ValidateAll();
        if (errors.Any())
            throw new InvalidOperationException(string.Join(" | ", errors));

        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }
}