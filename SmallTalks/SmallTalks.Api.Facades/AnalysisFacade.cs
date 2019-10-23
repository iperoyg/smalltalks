using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmallTalks.Api.Facades
{
    public class AnalysisFacade
    {

        public async Task<AnalysisResponseItem> AnalyseAsync()
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
                    var response = await AnalyseAsync(item, source.Token, '1');
                    _logger.Information("[{@SmallTalksAnalysisResult}] {@Sentence} analysed with CheckDate={@CheckDate} and InfoLevel={@InfoLevel}. Response: {@AnalysisResponse}", "Success", text, checkDate, infoLevel, response);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[{@SmallTalksAnalysisResult}] Unexpected fail when analysing sentence: {@Sentence}, {@CheckDate}, {@InfoLevel}", "Error", text, checkDate, infoLevel);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
