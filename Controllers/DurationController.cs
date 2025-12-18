using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using static Moldovan_Paula_Lab4.DurationPredictionModel;

namespace Moldovan_Paula_Lab4.Controllers
{
    public class DurationController : Controller
    {
        public IActionResult Estimate(ModelInput input)
        {
            MLContext mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(@"DurationPredictionModel.mlnet", out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel); 
            ModelOutput result = predEngine.Predict(input); 
            ViewBag.Duration = result.Score; 
            return View(input); 
        }
    }
}
