using Microsoft.AspNetCore.Mvc;
using BadWordsBackend.Services;

namespace BadWordsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly PredictionService _predictionService;

        public PredictionController(PredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost]
        public IActionResult Predict([FromBody] PredictionRequest request)
        {
            var result = _predictionService.Predict(request.Text);
            return Ok(new { text = request.Text, result });
        }
    }

    public class PredictionRequest
    {
        public string Text { get; set; }
    }
}
