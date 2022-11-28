using Microsoft.Bot.Builder.AI.Luis;

namespace ClinicBot.Infrastructure.Luis
{
    public interface ILuisService
    {
       LuisRecognizer _luisRecognizer { get; set; }
    }
}
