using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Moldovan_Paula_Lab4.Models;

namespace Moldovan_Paula_Lab4.Controllers
{
    public class MovieController : Controller
    {
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
            ViewBag.Score = result.Score.ToString().Equals("NaN") ? 0 : result.Score;
            ViewBag.MovieId = movieRatingData.movieId;
            ViewBag.UserId = movieRatingData.userId;

            return View(movieRatingData);
        }
    }
}
