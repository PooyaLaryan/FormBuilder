namespace FormBuilder.Components;

public class CardComponent : FormComponent
{
    public CardData Data { get; set; } = new();

    public override IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(Data.ImageUrl))
            yield return "تصویر کارت الزامی است.";
        if (string.IsNullOrWhiteSpace(Data.Text))
            yield return "متن کارت الزامی است.";
    }
}

public class CardData
{
    public string? ImageUrl { get; set; }
    public string? Text { get; set; }
    public ButtonData? Button { get; set; }
}

public class ButtonData
{
    public string Title { get; set; } = default!;
    public string ActionType { get; set; } = "link"; // link | deeplink | api
    public string? ActionValue { get; set; }
}