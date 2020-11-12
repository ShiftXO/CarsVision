namespace CarsVision.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using Newtonsoft.Json;

    public class MakesModelsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Makes.Any())
            {
                return;
            }

            StreamReader reader = new StreamReader(@"wwwroot\cars.json");

            string json = await reader.ReadToEndAsync();
            var jsonCars = JsonConvert.DeserializeObject<List<JsonCarDTO>>(json);

            var makesList = new List<Make>();

            foreach (var entry in jsonCars)
            {
                var make = new Make
                {
                    Name = entry.Brand,
                };

                foreach (var m in entry.Models)
                {
                    var model = new Model
                    {
                        Name = m,
                        MakeId = make.Id,
                    };

                    make.Models.Add(model);
                }

                makesList.Add(make);
            }

            await dbContext.Makes.AddRangeAsync(makesList);
            await dbContext.SaveChangesAsync();
        }
    }
}
