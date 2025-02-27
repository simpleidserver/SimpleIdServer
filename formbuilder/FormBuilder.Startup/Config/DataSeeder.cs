using FormBuilder.EF;
using FormBuilder.Startup.Workflows;
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
            form.AvailableStyles.Add(new FormBuilder.Models.FormStyle
            {
                Id = Guid.NewGuid().ToString(),
                Content = content,
                IsActive = true
            });
        }

        dbContext.Forms.AddRange(allForms);
    }

    private static void SeedWorkflows(FormBuilderDbContext dbContext)
    {
        dbContext.Workflows.Add(AuthWorkflows.SampleWorkflow);
        dbContext.Workflows.Add(AuthWorkflows.AuthWorkflow);
    }
}
