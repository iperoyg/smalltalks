using DateTimeDectector.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;
using Lime.Protocol;
using Newtonsoft.Json;
using Lime.Protocol.Serialization.Newtonsoft;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;

namespace DateTimeDectector.Core
{
    public class WatsonDatetimeDetector : IDateTimeDectector
    {
        private HttpClient _client = new HttpClient();
        private JsonNetSerializer _envelopeSerializer;

        public WatsonDatetimeDetector(IConfiguration configuration)
        {
            _envelopeSerializer = new JsonNetSerializer();
            _client.BaseAddress = new Uri("https://msging.net");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", configuration["BotKey"]);
        }

        public List<DateTimeDectected> Detect(string input)
        {
            return DetectAsync(input, CancellationToken.None).Result;
        }

        public async Task<List<DateTimeDectected>> DetectAsync(string input, CancellationToken cancellationToken)
        {
            var response = await AnalyseAsync(input, cancellationToken);
            if (response == null || response.Entities == null)
                return null;

            var entities = response.Entities
                .Select(e => EntityResponseToDateTimeDectected(e))
                .Where(e => e != null)
                .ToList();
            
            return entities;
        }

        private async Task<AnalysisResponse> AnalyseAsync(string input, CancellationToken cancellationToken)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var command = new Command
            {
                Id = EnvelopeId.NewId(),
                To = Node.Parse("postmaster@ai.msging.net"),
                Uri = new LimeUri("/analysis"),
                Method = CommandMethod.Set,
                Resource = new AnalysisRequest
                {
                    TestingRequest = true,
                    Text = input
                }
            };

            var envelopeResult = await RunCommandAsync(command, cancellationToken);
            return envelopeResult.Resource as AnalysisResponse;

        }

        private async Task<Command> RunCommandAsync(Command command, CancellationToken cancellationToken)
        {
            var commandString = _envelopeSerializer.Serialize(command);

            using (var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json"))
            {

                var response = await _client.PostAsync("/commands", httpContent, cancellationToken);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return (Command)_envelopeSerializer.Deserialize(responseBody);
            }
        }

        private DateTimeDectected EntityResponseToDateTimeDectected(EntityResponse entity)
        {
            if (entity.Id == "sys-date")
            {
                return new DateTimeDectected
                {
                    DetectionType = DateTimeParserDetectionType.SPECIFIC_DATE,
                    DateTime = DateTime.Parse(entity.Value)
                };
            }
            else if(entity.Id == "sys-time")
            {
                return new DateTimeDectected
                {
                    DetectionType = DateTimeParserDetectionType.SPECIFIC_TIME,
                    DateTime = DateTime.Parse(entity.Value)
                };
            }
            return null;
        }


        
    }
}
