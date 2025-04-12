using FormBuilder.EF;
using FormBuilder.Models;
using FormBuilder.Startup.Workflows;
using FormBuilder.Tailwindcss;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Startup.Config;

public class DataSeeder
{
    public static void SeedData(WebApplication webApplication)
    {
        using (var scope = webApplication.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            using (var dbContext = scope.ServiceProvider.GetService<FormBuilderDbContext>())
            {
                var isInMemory = dbContext.Database.IsInMemory();
                if (!isInMemory)
                {
                    dbContext.Database.Migrate();
                }

                SeedWorkflows(dbContext);
                SeedForms(dbContext);
                dbContext.SaveChanges();
            }
        }
    }

    private static void SeedForms(FormBuilderDbContext dbContext)
    {
        var content = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "form.css"));
        var allForms = AllForms.GetAllForms();
        foreach (var form in allForms)
        {
            var styles = new List<FormStyle>();
            // Enable radzen.
            // form.AvailableStyles.AddRange(RadzenTemplate.BuildDefault());
            // Enable tailwindcs : https://flowbite.com/blocks/marketing/login/
            form.AvailableStyles.Add(new FormStyle
            {
                Category = FormStyleCategories.Lib,
                Value = "https://cdn.jsdelivr.net/npm/@tailwindcss/browser@4",
                Id = Guid.NewGuid().ToString(),
                IsActive = true,
                TemplateName = TailwindCssTemplate.Name,
                Language = FormStyleLanguages.Javascript
            });
            // form.AvailableStyles.Add(new FormStyle
            // {
            //     Id = Guid.NewGuid().ToString(),
            //     Value = content,
            //     IsActive = true,
            //     TemplateName = TailwindCssTemplate.Name,
            //     Category = FormStyleCategories.Custom
            // });
        }

        dbContext.Forms.AddRange(allForms);
    }

    private static void SeedWorkflows(FormBuilderDbContext dbContext)
    {
        dbContext.Workflows.Add(AuthWorkflows.SampleWorkflow);
        dbContext.Workflows.Add(AuthWorkflows.AuthWorkflow);
    }
}
