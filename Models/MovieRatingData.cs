using Microsoft.ML.Data;

namespace Moldovan_Paula_Lab4.Models
{
    public class MovieRatingData
    {
        [LoadColumn(0)]
        public float userId;
        [LoadColumn(1)]
        public float movieId;
        [LoadColumn(2)]
        public float Label;
    }
}
