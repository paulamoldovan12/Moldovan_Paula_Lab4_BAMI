using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Moldovan_Paula_Lab4.Models;
using Moldovan_Paula_Lab4.Data;

namespace Moldovan_Paula_Lab4.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbContext _context; 
        public MovieController(AppDbContext context) 
        { 
            _context = context; 
        }

        public IActionResult Recommend(float userId, float movieId)
        {
            MLContext mlContext = new MLContext();

            string mlModelPath = @"MovieRecommenderModel.mlnet";
            ITransformer mlModel = mlContext.Model.Load(mlModelPath, out _);

            PredictionEngine<MovieRatingData, MovieRatingPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRatingData, MovieRatingPrediction>(mlModel);

            MovieRatingData movieRatingData = new MovieRatingData()
            {
                userId = userId,
                movieId = movieId,
            };
            MovieRatingPrediction result = predictionEngine.Predict(movieRatingData);
            var history = new MoviePredictionHistory 
            { 
                UserId = userId, 
                MovieId = movieId, 
                Score = result.Score, 
                Timestamp = DateTime.Now 
            }; 
            _context.MoviePredictionHistory.Add(history); 
            _context.SaveChanges();

            ViewBag.Score = result.Score.ToString().Equals("NaN") ? 0 : result.Score;
            ViewBag.MovieId = movieRatingData.movieId;
            ViewBag.UserId = movieRatingData.userId;

            return View(movieRatingData);
        }
    }
}
