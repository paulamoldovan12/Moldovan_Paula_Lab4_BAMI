using Microsoft.AspNetCore.Mvc;
using Moldovan_Paula_Lab4.Data; 
using Moldovan_Paula_Lab4.Models;

namespace Moldovan_Paula_Lab4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionApiController : Controller
    {
        private readonly AppDbContext _context; 
        public PredictionApiController(AppDbContext context) 
        { 
            _context = context; 
        } 

        // GET: api/predictionapi
        [HttpGet]
        public IActionResult GetAll() 
        { 
            var list = _context.MoviePredictionHistory.ToList(); 
            return Ok(list); 
        } 
        
        // DELETE: api/predictionapi/5
        [HttpDelete("{id}")] 
        public IActionResult Delete(int id) 
        { 
            var item = _context.MoviePredictionHistory.Find(id); 

            if (item == null) 
                return NotFound(); 
            
            _context.MoviePredictionHistory.Remove(item); 
            _context.SaveChanges(); 
            return Ok(new { message = "Șters cu succes" }); 
        }
    }
}
