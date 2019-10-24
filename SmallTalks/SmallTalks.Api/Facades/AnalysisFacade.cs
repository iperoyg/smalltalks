using SmallTalks.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SmallTalks.Core.Models;
using System.Threading;
using SmallTalks.Core;
using DateTimeDectector.Domain;
using System.Diagnostics;
using SmallTalks.Api.Facades.Interfaces;
using System.Threading.Tasks.Dataflow;

namespace SmallTalks.Api.Facades
{
    public class AnalysisFacade : IAnalysisFacade
    {
        private readonly ILogger _logger;
        private readonly ISmallTalksDetector _smallTalksDetector;
        private readonly IDateTimeDectector _dateTimeDectector;

        public AnalysisFacade(
            ILogger logger,
            ISmallTalksDetector smallTalksDetector,
            IDateTimeDectector dateTimeDectector)
        {
            _logger = logger;
            _smallTalksDetector = smallTalksDetector;
            _dateTimeDectector = dateTimeDectector;
        }

        public async Task<AnalysisResponseItem> AnalyseAsync(string text, bool checkDate = true, int infoLevel = 1)
        {
            var isTextInvalid = string.IsNullOrEmpty(text);
            var isInfoLevelInvalid = !Enum.IsDefined(typeof(InformationLevel), infoLevel);

            if (isTextInvalid || isInfoLevelInvalid)
            {
                var param = isTextInvalid ? nameof(text) : nameof(infoLevel);
                _logger.Warning("{@Text} or {@InfoLevel} constraints violated!", text, infoLevel);
                throw new ArgumentException("Invalid Text or InfoLevel. Check restrictions.", param);
            }

            var item = new ConfiguredAnalysisRequestItem
            {
                Text = text,
                CheckDate = checkDate,
                Configuration = new SmallTalksPreProcessingConfiguration()
                {
                    InformationLevel = (InformationLevel)infoLevel
                }
            };

            return await GetAnalysisResponseAsync(item);
        }

        public async Task<AnalysisResponseItem> ConfiguredAnalyseAsync(ConfiguredAnalysisRequestItem requestItem)
        {
            if (requestItem == null || string.IsNullOrEmpty(requestItem.Text))
            {
                throw new ArgumentException("Invalid request item or content!");
            }

            return await GetAnalysisResponseAsync(requestItem);
        }

        public async Task<BatchAnalysisResponse> BatchAnalyse(BatchAnalysisRequest request)
        {

            if (request == null || !request.IsValid())
            {
                throw new ArgumentException("Batch Analysis request object not valid");
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

            tranformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var item in request.Items)
            {
                await tranformBlock.SendAsync(item);
            }

            tranformBlock.Complete();
            await actionBlock.Completion;

            return response;
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

        private async Task<AnalysisResponseItem> GetAnalysisResponseAsync(ConfiguredAnalysisRequestItem requestItem)
        {
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                var response = await AnalyseAsync(requestItem, source.Token, ApiVersion.V1);
                _logger.Information(response.ToString());
                return response;
            }
        }

        private async Task<AnalysisResponseItem> AnalyseAsync(ConfiguredAnalysisRequestItem item, CancellationToken cancellationToken, ApiVersion apiVersion)
        {
            var analysisResponse = new AnalysisResponseItem
            {
                Id = item.Id,
                SmallTalksAnalysis = apiVersion == ApiVersion.V2 ? await _smallTalksDetector.DetectAsyncv2(item.Text, item.Configuration)
                                                                 : await _smallTalksDetector.DetectAsync(item.Text, item.Configuration),
                DateTimeDectecteds = await DetectDateAsync(item, cancellationToken),
            };

            return analysisResponse;
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
    }
}
