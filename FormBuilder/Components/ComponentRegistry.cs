namespace FormBuilder.Components;

public record ComponentDescriptor(string Type, string DisplayName, string Description);

public static class ComponentRegistry
{
    public static readonly IReadOnlyList<ComponentDescriptor> All = new List<ComponentDescriptor>
    {
        new("image", "تصویر",    "فقط یک تصویر"),
        new("poll",  "نظرسنجی",  "سوال و گزینه‌ها"),
        new("card",  "کارت",     "تصویر + متن + دکمه"),
    };

    // ساخت کامپوننت خالی برای افزودن به صفحه
    public static FormComponent CreateEmpty(string type) => type switch
    {
        "image" => new ImageComponent(),
        "poll" => new PollComponent(),
        "card" => new CardComponent(),
        _ => throw new ArgumentException($"Unknown component type: {type}")
    };
}