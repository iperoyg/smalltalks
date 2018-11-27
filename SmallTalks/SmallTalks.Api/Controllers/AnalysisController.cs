using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DateTimeDectector.Domain;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmallTalks.Api.Models;
using SmallTalks.Core;
using SmallTalks.Core.Models;

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
        /// <param name="text">Text to be analysed (cannot be null or empty) </param>
        /// <param name="checkDate">Optional. Default: true. Make a datetime check</param>
        /// <param name="infoLevel">Optional. Default: 1. Controls the amount of information delivered in JSON (1 - minimum, 2 - normal, 3 - full)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Analyse(string text, bool checkDate = true, int infoLevel = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(text) || infoLevel < 1 || infoLevel > 3)
                {
                    _logger.Warning("{@Text} or {@InfoLevel} constraints violated!", text, infoLevel);
                    return BadRequest(new { message = "Check 'text' and 'infoLevel' restrictions" });
                }

                var item = new ConfiguredAnalysisRequestItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = text,
                    CheckDate = checkDate,
                    Configuration = new SmallTalksPreProcessingConfiguration()
                    {
                        InformationLevel = (InformationLevel)infoLevel
                    }
                };

                using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                {
                    var response = await AnalyseAsync(item, source.Token);
                    _logger.Information("[{@SmallTalksAnalysisResult}] {@Sentence} analysed with CheckDate={@CheckDate} and InfoLevel={@InfoLevel}. Response: {@AnalysisResponse}", "Success", text, checkDate, infoLevel, response);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[{@SmallTalksAnalysisResult}] Unexpected fail when analysing sentence: {@Sentence}, {@CheckDate}, {@InfoLevel}", "Error", text, checkDate, infoLevel);
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfiguredAnalyse([FromBody] ConfiguredAnalysisRequestItem requestItem)
        {
            try
            {
                if (requestItem == null || string.IsNullOrEmpty(requestItem.Text))
                {
                    return BadRequest();
                }

                using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                {
                    var response = await AnalyseAsync(requestItem, source.Token);
                    _logger.Information(response.ToString());
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected fail when analysing sentence: {requestItem}", requestItem);
                return StatusCode(500, ex);
            }
        }

        private async Task<List<DateTimeDectected>> DetectDateAsync(ConfiguredAnalysisRequestItem requestItem, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return requestItem.CheckDate ? await _dateTimeDectector.DetectAsync(requestItem.Text, cancellationToken) : null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Couldn't check date for {@RequestItem}", requestItem);
                return null;
            }
            finally
            {
                sw.Stop();
                _logger.Information("Finish date detect after {@ElapsedMillisecods} for {@RequestItem}", sw.ElapsedMilliseconds, requestItem);
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

                var tranformBlock = new TransformBlock<ConfiguredAnalysisRequestItem, AnalysisResponseItem>((Func<ConfiguredAnalysisRequestItem, Task<AnalysisResponseItem>>)UnsafeAnalyseAsync,
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
                _logger.Error(ex, "Unexpected fail when analysing sentence: {id}, {request}", request.Id, request);
                return StatusCode(500, ex);
            }
        }

        private async Task<AnalysisResponseItem> AnalyseAsync(ConfiguredAnalysisRequestItem item, CancellationToken cancellationToken)
        {
            var analysisResponse = new AnalysisResponseItem
            {
                Id = item.Id,
                SmallTalksAnalysis = await _smallTalksDetector.DetectAsync(item.Text, item.Configuration),
                DateTimeDectecteds = await DetectDateAsync(item, cancellationToken),
            };

            return analysisResponse;
        }

        private async Task<AnalysisResponseItem> UnsafeAnalyseAsync(ConfiguredAnalysisRequestItem item)
        {
            var analysisResponse = new AnalysisResponseItem
            {
                Id = item.Id,
                SmallTalksAnalysis = await _smallTalksDetector.DetectAsync(item.Text, item.Configuration),
                DateTimeDectecteds = await DetectDateAsync(item, CancellationToken.None),
            };

            return analysisResponse;
        }

        private bool ValidateBatchAnalysis(BatchAnalysisRequest request)
        {
            return request != null && request.Items != null && request.Items.Count > 0 && !string.IsNullOrEmpty(request.Id);
        }


    }
}
