namespace FormBuilder.Components;

public class PollComponent : FormComponent
{
    public PollData Data { get; set; } = new();

    public override IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(Data.Question))
            yield return "سوال نظرسنجی الزامی است.";
        if (Data.Options.Count < 2)
            yield return "نظرسنجی حداقل به دو گزینه نیاز دارد.";
    }
}

public class PollData
{
    public string? Question { get; set; }
    public bool AllowMultiple { get; set; }
    public List<PollOption> Options { get; set; } = new();
}

public class PollOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = default!;
}