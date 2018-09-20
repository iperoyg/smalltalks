using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AnalysisController(ISmallTalksDetector smallTalksDetector)
        {
            _smallTalksDetector = smallTalksDetector;
        }

        /// <summary>
        /// Analyses a text to check for small talks
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Analyse(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                return BadRequest();
            }

            var analysis = _smallTalksDetector.Detect(text);
            return Ok(analysis);
        }

    }
}