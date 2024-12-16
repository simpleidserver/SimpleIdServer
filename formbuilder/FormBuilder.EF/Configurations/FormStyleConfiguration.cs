using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormBuilder.EF.Configurations;

public class FormStyleConfiguration : IEntityTypeConfiguration<FormStyle>
{
    public void Configure(EntityTypeBuilder<FormStyle> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
