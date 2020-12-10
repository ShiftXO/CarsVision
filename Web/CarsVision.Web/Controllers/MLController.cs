namespace CarsVision.Web.Controllers
{
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVisionML.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.ML;

    public class MLController : Controller
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> predictionEngine;
        private readonly IMLService mLService;

        public MLController(PredictionEnginePool<ModelInput, ModelOutput> predictionEngine, IMLService mLService)
        {
            this.predictionEngine = predictionEngine;
            this.mLService = mLService;
        }

        public IActionResult Predict(CarPredictInputModel input)
        {
            var model = this.mLService.GetValues(input);

            var modelInput = new ModelInput
            {
                Make = model.Make,
                Model = model.Model,
                Year = model.Year,
                Gearbox = model.Gearbox,
                Mileage = model.Mileage,
                Power = model.Power,
                Eurostandard = model.Eurostandard,
            };

            var output = this.predictionEngine.Predict(modelInput);

            return this.Content(output.Score.ToString());
        }
    }
}
