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

namespace DateTimeDectector.Core
{
    public class WatsonDatetimeDetector : IDateTimeDectector
    {
        private HttpClient _client = new HttpClient();
        private JsonNetSerializer _envelopeSerializer;

        public WatsonDatetimeDetector()
        {
            _envelopeSerializer = new JsonNetSerializer();
            _client.BaseAddress = new Uri("https://msging.net");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", "aW50ZWxpZ2VuY2lhYXByZW5kaXo6bzFuOFJoTW1xUkI3VVZsRWFxeTU=");
        }

        public List<DateTimeDectected> Detect(string input)
        {
            return DetectAsync(input).Result;
        }

        public async Task<List<DateTimeDectected>> DetectAsync(string input)
        {
            var response = await AnalyseAsync(input);
            return null;
        }

        public async Task<AnalysisResponse> AnalyseAsync(string input)
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

            var envelopeResult = await RunCommandAsync(command);
            return envelopeResult.Resource as AnalysisResponse;

        }

        private async Task<Command> RunCommandAsync(Command command)
        {
            var commandString = _envelopeSerializer.Serialize(command);

            using (var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json"))
            {

                var response = await _client.PostAsync("/commands", httpContent);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return (Command)_envelopeSerializer.Deserialize(responseBody);
            }
        }

        List<DateTimeDectected> IDateTimeDectector.Detect(string input)
        {
            throw new NotImplementedException();
        }

        
    }
}
