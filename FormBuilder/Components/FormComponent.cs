using System.Text.Json.Serialization;

namespace FormBuilder.Components;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ImageComponent), "image")]
[JsonDerivedType(typeof(PollComponent), "poll")]
[JsonDerivedType(typeof(CardComponent), "card")]   // عکس + متن + دکمه
public abstract class FormComponent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Order { get; set; }
    public bool IsVisible { get; set; } = true;

    // برای اینکه قبل از انتشار بررسی کنیم داده‌ها کامل پر شده‌اند
    public abstract IEnumerable<string> Validate();
}