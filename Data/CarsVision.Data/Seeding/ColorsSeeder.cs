namespace CarsVision.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using Newtonsoft.Json;

    public class ColorsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Colors.Any())
            {
                return;
            }

            StreamReader reader = new StreamReader(@"wwwroot\colors.json");

            string json = await reader.ReadToEndAsync();
            var jsonColors = JsonConvert.DeserializeObject<JsonColorDTO>(json);

            var colors = new List<Color>();

            foreach (var entry in jsonColors.Colors)
            {
                var color = new Color
                {
                    Name = entry,
                };

                colors.Add(color);
            }

            await dbContext.Colors.AddRangeAsync(colors);
            await dbContext.SaveChangesAsync();
        }
    }
}
