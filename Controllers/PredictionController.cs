using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Moldovan_Paula_Lab4.Data;
using Moldovan_Paula_Lab4.Models;
using static Moldovan_Paula_Lab4.PricePredictionModel;

namespace Moldovan_Paula_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        private readonly AppDbContext _context; 
        public PredictionController(AppDbContext context) 
        { 
            _context = context; 
        }

        [HttpPost]
        public async Task<IActionResult> Price(ModelInput input)
        {
            // Load the model   
            MLContext mlContext = new MLContext();
            // Create predection engine related to the loaded train model   
            ITransformer mlModel =
            mlContext.Model.Load(@"PricePredictionModel.mlnet", out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput,
            ModelOutput>(mlModel);
            // Try model on sample data to predict fair price   
            ModelOutput result = predEngine.Predict(input);
            ViewBag.Price = result.Score;

            var history = new PredictionHistory
            {
                PassengerCount = input.Passenger_count,
                TripTimeInSecs = input.Trip_time_in_secs,
                TripDistance = input.Trip_distance,
                PaymentType = input.Payment_type,
                PredictedPrice = result.Score,
                CreatedAt = DateTime.Now
            };

            _context.PredictionHistories.Add(history);
            await _context.SaveChangesAsync();

            return View(input);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _context.PredictionHistories
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(history);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // 1. Numărul total de predicții 
            var totalPredictions = await _context.PredictionHistories.CountAsync();

            // 2. Preț mediu per tip de plată + număr de predicții per tip 
            var paymentTypeStats = await _context.PredictionHistories
                .GroupBy(p => p.PaymentType)
                .Select(g => new PaymentTypeStat
                {
                    PaymentType = g.Key,
                    AveragePrice = g.Average(x => x.PredictedPrice),
                    Count = g.Count()
                })
                .ToListAsync();

            // 3. Distribuția prețurilor pe intervale (buckets) 
            // Definim intervalele: 0-10, 10-20, 20-30, 30-50, >50 (exemplu) 
            var allPredictions = await _context.PredictionHistories
                .Select(p => p.PredictedPrice)
                .ToListAsync();

            var buckets = new List<PriceBucketStat>
        {
            new PriceBucketStat { Label = "0 - 10" },
            new PriceBucketStat { Label = "10 - 20" },
            new PriceBucketStat { Label = "20 - 30" },
            new PriceBucketStat { Label = "30 - 50" },
            new PriceBucketStat { Label = "> 50" }
        };

            foreach (var price in allPredictions)
            {
                if (price < 10)
                    buckets[0].Count++;
                else if (price < 20)
                    buckets[1].Count++;
                else if (price < 30)
                    buckets[2].Count++;
                else if (price < 50)
                    buckets[3].Count++;
                else
                    buckets[4].Count++;
            }

            // 4. Construim ViewModel-ul 
            var vm = new DashboardViewModel
            {
                TotalPredictions = totalPredictions,
                PaymentTypeStats = paymentTypeStats,
                PriceBuckets = buckets
            };

            return View(vm);
        }
    }
}
