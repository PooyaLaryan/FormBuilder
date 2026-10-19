using FormBuilder.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace FormBuilder.Pages;

public class FormPageConfiguration : IEntityTypeConfiguration<FormPage>
{
    private static readonly JsonSerializerOptions Options =
    new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<FormPage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.Slug).HasMaxLength(100);

        builder.Property(x => x.Components)
            .HasConversion(
                components => JsonSerializer.Serialize(components, Options),
                json => JsonSerializer.Deserialize<List<FormComponent>>(
                    json, Options) ?? new List<FormComponent>())
            .HasColumnType("nvarchar(max)")
            .Metadata.SetValueComparer(
                new ValueComparer<List<FormComponent>>(
                    (a, b) => JsonSerializer.Serialize(a, Options) ==
                              JsonSerializer.Serialize(b, Options),
                    v => JsonSerializer.Serialize(v, Options).GetHashCode(),
                    v => JsonSerializer.Deserialize<List<FormComponent>>(
                        JsonSerializer.Serialize(v, Options), Options)!
                ));
    }

}