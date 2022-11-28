using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.LuisV3;
using Microsoft.Extensions.Configuration;
using LuisApplication = Microsoft.Bot.Builder.AI.Luis.LuisApplication;
using LuisRecognizer = Microsoft.Bot.Builder.AI.Luis.LuisRecognizer;

namespace ClinicBot.Infrastructure.Luis
{
    public class LuisService: ILuisService
    {
        public LuisRecognizer _luisRecognizer { get; set; }

        public LuisService(IConfiguration configuration)
        {
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisApiKey"],
                configuration["LuisHostName"]
            );

            var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
            {
                PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions()
                {
                    IncludeInstanceData = true
                }
            };
            _luisRecognizer = new LuisRecognizer(recognizerOptions);
        }
    }
}
