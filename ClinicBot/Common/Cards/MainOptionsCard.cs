using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Common.Cards
{
    public class MainOptionsCard
    {
        public static async Task ToShow(DialogContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(activity: CreateCarrucel(), cancellationToken);
        }

        private static Activity CreateCarrucel()
        {
            var cardCitasMedicas = new HeroCard()
            {
                Title = "Citas Medicas",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/menu_01.jpg") },
                Buttons = new List<CardAction>() 
                {
                    new CardAction(){Title = "Crear cita medica", Value = "Crear cita medica", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Ver cita medica", Value = "Ver cita medica", Type = ActionTypes.ImBack},

                }
            };

            var cardInformacionContacto= new HeroCard()
            {
                Title = "Informacion de contacto",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/menu_02.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Centro de contacto", Value = "Centro de contact", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Sitio web", Value = "https://docs.microsoft.com/", Type = ActionTypes.OpenUrl},

                }
            };

            var cardSiguenosRedes = new HeroCard()
            {
                Title = "Siguenos en las redes",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/menu_03.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Facebook", Value = "https://www.facebook.com", Type = ActionTypes.OpenUrl},
                    new CardAction(){Title = "Instagram", Value = "https://www.instagram.com", Type = ActionTypes.OpenUrl},
                    new CardAction(){Title = "Twitter", Value = "https://www.twitter.com", Type = ActionTypes.OpenUrl}
                }
            };

            var cardCalification = new HeroCard()
            {
                Title = "Calificacion",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/menu_04.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Calificar Bot", Value = "Calificar Bot", Type = ActionTypes.ImBack}
                }
            };

            var optionsAttachments = new List<Attachment>()
            {
                cardCitasMedicas.ToAttachment(),
                cardInformacionContacto.ToAttachment(),
                cardSiguenosRedes.ToAttachment(),
                cardCalification.ToAttachment(),
            };

            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;
        }
    }
}
