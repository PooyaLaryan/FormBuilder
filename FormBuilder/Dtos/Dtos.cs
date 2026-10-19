using FormBuilder.Components;

namespace FormBuilder.Dtos;

public record CreatePageRequest(string Slug, string Title);

public record UpdatePageRequest(string Title, List<FormComponent> Components);

public record PageSummary(Guid Id, string Slug, string Title, bool IsPublished, int ComponentCount);