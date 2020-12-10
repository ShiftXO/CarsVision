// This file was auto-generated by ML.NET Model Builder. 

using System;
using CarsVisionML.Model;

namespace CarsVisionML.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = new ModelInput()
            {
                Make = @"Mercedes-Benz",
                Model = @"E 350",
                Year = 2013F,
                Power = 251F,
                Mileage = 200000F,
                Eurostandard = @"EURO 6",
                Gearbox = @"Автоматични скорости",
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual Price with predicted Price from sample data...\n\n");
            Console.WriteLine($"Make: {sampleData.Make}");
            Console.WriteLine($"Model: {sampleData.Model}");
            Console.WriteLine($"Year: {sampleData.Year}");
            Console.WriteLine($"Power: {sampleData.Power}");
            Console.WriteLine($"Mileage: {sampleData.Mileage}");
            Console.WriteLine($"Eurostandard: {sampleData.Eurostandard}");
            Console.WriteLine($"Gearbox: {sampleData.Gearbox}");
            Console.WriteLine($"\n\nPredicted Price: {predictionResult.Score}\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}
