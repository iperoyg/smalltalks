using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DateTimeDectector.Domain;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
        private readonly ILogger _logger;

        public AnalysisController(
            ISmallTalksDetector smallTalksDetector,
            IDateTimeDectector dateTimeDectector,
            ILogger logger)
        {
            _smallTalksDetector = smallTalksDetector;
            _dateTimeDectector = dateTimeDectector;
            _logger = logger;
        }

        /// <summary>
        /// Analyses a text to check for small talks and datetimes
        /// </summary>
        /// <param name="text">Text to be analysed</param>
        /// <param name="checkDate">Optional. Default: true. Make a datetime check</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Analyse(string text, bool checkDate = true)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return BadRequest();
                }

                var response = new AnalysisResponseItem
                {
                    SmallTalksAnalysis = await _smallTalksDetector.DetectAsync(text),
                    DateTimeDectecteds = checkDate ? await _dateTimeDectector.DetectAsync(text) : null
                };

                _logger.Information(response.ToString());
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {sentence}", text);
                return StatusCode(500, ex);
            }
        }

        [HttpPost, Route("batch")]
        public async Task<IActionResult> BatchAnalyse([FromBody] BatchAnalysisRequest request)
        {
            try
            {
                if (!ValidateBatchAnalysis(request))
                {
                    return BadRequest();
                }

                var response = new BatchAnalysisResponse
                {
                    Items = new List<AnalysisResponseItem>(),
                    Id = request.Id
                };

                var tranformBlock = new TransformBlock<AnalysisRequestItem, AnalysisResponseItem>((Func<AnalysisRequestItem, Task<AnalysisResponseItem>>)AnalyseAsync,
                    new ExecutionDataflowBlockOptions
                    {
                        BoundedCapacity = ExecutionDataflowBlockOptions.Unbounded,
                        MaxDegreeOfParallelism = 100
                    });
                var actionBlock = new ActionBlock<AnalysisResponseItem>(i =>
                {
                    response.Items.Add(i);
                }, new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = ExecutionDataflowBlockOptions.Unbounded,
                });
                tranformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true, });

                foreach (var item in request.Items)
                {
                    await tranformBlock.SendAsync(item);
                }
                tranformBlock.Complete();
                await actionBlock.Completion;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {sentence}", request.Id);
                return StatusCode(500, ex);
            }
        }

        private async Task<AnalysisResponseItem> AnalyseAsync(AnalysisRequestItem item)
        {
            return new AnalysisResponseItem
            {
                Id = item.Id,
                SmallTalksAnalysis = await _smallTalksDetector.DetectAsync(item.Text),
                DateTimeDectecteds = item.DateCheck ? await _dateTimeDectector.DetectAsync(item.Text) : null
            };
        }

        private bool ValidateBatchAnalysis(BatchAnalysisRequest request)
        {
            return request != null && request.Items != null && request.Items.Count > 0 && !string.IsNullOrEmpty(request.Id);
        }


    }
}
