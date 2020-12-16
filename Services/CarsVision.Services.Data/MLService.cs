namespace CarsVision.Services.Data
{
    using CarsVision.Web.ViewModels.Cars;

    public class MLService : IMLService
    {
        public MLCarDTO GetValues(CarPredictInputModel input)
        {
            var euro = input.EuroStandard.ToString().Replace("Euro_", "EURO ");
            var gearbox = input.Gearbox.ToString();
            gearbox = gearbox == "Manual" ? "Ръчни скорости" : "Автоматични скорости";

            var obj = new MLCarDTO
            {
                Make = input.Make,
                Model = input.Model,
                Year = int.Parse(input.Year),
                Mileage = input.Mileage,
                Power = input.Power,
                Eurostandard = euro,
                Gearbox = gearbox,
            };

            return obj;
        }
    }
}
