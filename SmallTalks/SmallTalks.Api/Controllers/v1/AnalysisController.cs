using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DateTimeDectector.Domain;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmallTalks.Api.Facades.Interfaces;
using SmallTalks.Api.Filters;
using SmallTalks.Api.Models;
using SmallTalks.Core;
using SmallTalks.Core.Models;

namespace SmallTalks.Api.Controllers.v1
{
    /// <summary>
    /// Controller responsible for analysing texts
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiVersion("1")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ISmallTalksDetector _smallTalksDetector;
        private readonly IDateTimeDectector _dateTimeDectector;
        private readonly ILogger _logger;
        private readonly IAnalysisFacade _analysisFacade;

        public AnalysisController(
            ISmallTalksDetector smallTalksDetector,
            IDateTimeDectector dateTimeDectector,
            ILogger logger,
            IAnalysisFacade analysisFacade)
        {
            _smallTalksDetector = smallTalksDetector;
            _dateTimeDectector = dateTimeDectector;
            _logger = logger;
            _analysisFacade = analysisFacade;
        }

        /// <summary>
        /// Analyses a SINGLE input for Smalltalks types and datetimes
        /// </summary>
        /// <param name="text">Text to be analysed *(cannot be null or empty)* </param>
        /// <param name="checkDate">Optional. Make a datetime check. *(If date is not desired, please set to false.)*</param>
        /// <param name="infoLevel">Optional. Controls the amount of information delivered in JSON (1 - minimum, 2 - normal, 3 - full)</param>
        /// <remarks>
        ///     **InfoLevel Description:**
        ///         **Lvl 1**
        ///             Smalltalk type identified
        ///             Cursed ----------------- *boolean index that returns true when a crused words is found on the input*
        ///         **Lvl 2**
        ///             Fields from **Lvl 1** plus:
        ///             Value ------------------- *the exact identified input*
        ///             MarketInput --------- *input with a placeholder in the place of **Value***
        ///             CleanedInput ------- *input without the **Value***
        ///             UseCleaned ---------- *boolean index that tells if the **CleanedInput** should be used or not*
        ///             CleanedRatio -------- *the ratio of how much was identified and deleted from the input*
        ///         **Lvl 3**
        ///             Fields from **Lvl 2** plus:
        ///             Index ------------------- *the identified value initial position*
        ///             Length ----------------- *the identified value length*
        ///             Relevant input ------ *input without stopwords* (**Beta**, not recommended)
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Successful API call. </response>
        [HttpGet]
        [ServiceFilter(typeof(CustomAuthenticationFilter))]
        [ProducesResponseType(typeof(AnalysisResponseItem), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Analyse([Required]string text, bool checkDate = true, int infoLevel = 1)
        {
            try
            {
                var response = await _analysisFacade.AnalyseAsync(text, checkDate, infoLevel);

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[{@SmallTalksAnalysisResult}] Unexpected fail when analysing sentence: {@Sentence}, {@CheckDate}, {@InfoLevel}", "Error", text, checkDate, infoLevel);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Analyses a JSON with a SINGLE input for Smalltalks types and datetimes
        /// </summary>
        /// <param name="requestItem">A JSON declaring the configuration parameters for the analysis, and a SINGLE input to be analysed</param>
        /// <returns></returns>
        /// <response code="200">Input successfully analysed. </response>
        /// <response code="400">Input is null or empty, or there is no items to be analysed. </response>
        /// <response code="500">Internal Error. Check exception and log.</response>"
        [HttpPost]
        [ServiceFilter(typeof(CustomAuthenticationFilter))]
        [ProducesResponseType(typeof(BatchAnalysisResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfiguredAnalyse([FromBody] ConfiguredAnalysisRequestItem requestItem)
        {
            try
            {
                var response = await _analysisFacade.ConfiguredAnalyseAsync(requestItem);

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {requestItem}", requestItem);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Analyses a BATCH of inputs for Smalltalks types and datetimes
        /// </summary>
        /// <param name="request">A JSON declaring the configuration parameters for the analysis, and a BATCH of inputs to be analysed</param>
        /// <returns></returns>
        /// <response code="200">Batch successfully analysed. </response>
        /// <response code="400">Batch is null or empty, or there is no items to be analysed. </response>
        /// <response code="500">Internal Error. Check exception and log.</response>"
        [HttpPost, Route("batch")]
        [ServiceFilter(typeof(CustomAuthenticationFilter))]
        [ProducesResponseType(typeof(BatchAnalysisResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BatchAnalyse([FromBody] BatchAnalysisRequest request)
        {
            try
            {
                var response = await _analysisFacade.BatchAnalyse(request);

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {id}, {request}", request.Id, request);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Analyses a SINGLE input for Smalltalks types and datetimes
        /// </summary>
        /// <param name="text">Text to be analysed *(cannot be null or empty)* </param>
        /// <param name="checkDate">Optional. Make a datetime check. *(If date is not desired, please set to false.)*</param>
        /// <param name="infoLevel">Optional. Controls the amount of information delivered in JSON (1 - minimum, 2 - normal, 3 - full)</param>
        /// <remarks>
        ///     **InfoLevel Description:**
        ///         **Lvl 1**
        ///             Smalltalk type identified
        ///             Cursed ----------------- *boolean index that returns true when a crused words is found on the input*
        ///         **Lvl 2**
        ///             Fields from **Lvl 1** plus:
        ///             Value ------------------- *the exact identified input*
        ///             MarketInput --------- *input with a placeholder in the place of **Value***
        ///             CleanedInput ------- *input without the **Value***
        ///             UseCleaned ---------- *boolean index that tells if the **CleanedInput** should be used or not*
        ///             CleanedRatio -------- *the ratio of how much was identified and deleted from the input*
        ///         **Lvl 3**
        ///             Fields from **Lvl 2** plus:
        ///             Index ------------------- *the identified value initial position*
        ///             Length ----------------- *the identified value length*
        ///             Relevant input ------ *input without stopwords* (**Beta**, not recommended)
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Successful API call. </response>
        [HttpGet, Route("v2")]
        [ServiceFilter(typeof(CustomAuthenticationFilter))]
        [ProducesResponseType(typeof(AnalysisResponseItem), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AnalyseV2([Required]string text, bool checkDate = true, int infoLevel = 1)
        {
            try
            {
                var response = await _analysisFacade.AnalyseAsyncV2(text, checkDate, infoLevel);

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[{@SmallTalksAnalysisResult}] Unexpected fail when analysing sentence: {@Sentence}, {@CheckDate}, {@InfoLevel}", "Error", text, checkDate, infoLevel);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Analyses a JSON with a SINGLE input for Smalltalks types and datetimes
        /// </summary>
        /// <param name="requestItem">A JSON declaring the configuration parameters for the analysis, and a SINGLE input to be analysed</param>
        /// <returns></returns>
        /// <response code="200">Input successfully analysed. </response>
        /// <response code="400">Input is null or empty, or there is no items to be analysed. </response>
        /// <response code="500">Internal Error. Check exception and log.</response>"

        [HttpPost, Route("v2")]
        [ServiceFilter(typeof(CustomAuthenticationFilter))]
        [ProducesResponseType(typeof(BatchAnalysisResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfiguredAnalyseV2([FromBody] ConfiguredAnalysisRequestItem requestItem)
        {
            try
            {
                var response = await _analysisFacade.ConfiguredAnalyseAsyncV2(requestItem);

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {requestItem}", requestItem);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Analyses a BATCH of inputs for Smalltalks types and datetimes
        /// </summary>
        /// <param name="request">A JSON declaring the configuration parameters for the analysis, and a BATCH of inputs to be analysed</param>
        /// <returns></returns>
        /// <response code="200">Batch successfully analysed. </response>
        /// <response code="400">Batch is null or empty, or there is no items to be analysed. </response>
        /// <response code="500">Internal Error. Check exception and log.</response>"
        [HttpPost, Route("v2/batch")]
        [ServiceFilter(typeof(CustomAuthenticationFilter))]
        [ProducesResponseType(typeof(BatchAnalysisResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BatchAnalyseV2([FromBody] BatchAnalysisRequest request)
        {
            try
            {
                var response = await _analysisFacade.BatchAnalyseV2(request);

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {id}, {request}", request.Id, request);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
