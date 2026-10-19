namespace FormBuilder.Components;

public class ImageComponent : FormComponent
{
    public ImageData Data { get; set; } = new();

    public override IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(Data.Url))
            yield return "آدرس تصویر الزامی است.";
    }
}

public class ImageData
{
    public string? Url { get; set; }
    public string? Alt { get; set; }
}