using System.Threading.Tasks;
using DateTimeDectector.Domain;
using Microsoft.AspNetCore.Mvc;
using SmallTalks.Api.Models;
using SmallTalks.Core;

namespace SmallTalks.Api.Controllers
{
    /// <summary>
    /// Controller responsible for analysing texts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ISmallTalksDetector _smallTalksDetector;
        private readonly IDateTimeDectector _dateTimeDectector;

        public AnalysisController(ISmallTalksDetector smallTalksDetector, IDateTimeDectector dateTimeDectector)
        {
            _smallTalksDetector = smallTalksDetector;
            _dateTimeDectector = dateTimeDectector;
            
        }

        /// <summary>
        /// Analyses a text to check for small talks and datetimes
        /// </summary>
        /// <param name="text">Text to be analysed</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Analyse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest();
            }

            var response = new AnalysisResponse
            {
                SmallTalksAnalysis = await _smallTalksDetector.DetectAsync(text),
                DateTimeDectecteds = await _dateTimeDectector.DetectAsync(text)
            };

            return Ok(response);
        }


    }
}
