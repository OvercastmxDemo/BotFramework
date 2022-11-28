using ClinicBot.Common.Cards;
using ClinicBot.Infrastructure.Luis;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Dialog
{
    public class RootDialog: ComponentDialog
    {
        private readonly ILuisService _luisService;

        public RootDialog(ILuisService luisService)
        {
            _luisService = luisService;

            var waterfallSteps = new WaterfallStep[]
            {
                InitialProcess,
                FinalProcess
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntention(stepContext, luisResult, cancellationToken);
        }

        private async Task<DialogTurnResult> ManageIntention(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent) {
                
                case "Saludar":
                    await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case "Despedir":
                    await IntentDespedir(stepContext, luisResult, cancellationToken);
                    break;
                case "VerOpciones":
                    await IntentVerOpciones(stepContext, luisResult, cancellationToken);
                    break;
                case "None":
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                default:
                    break;
            }

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }
        private async void button1_Click(object sender, EventArgs e, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "ein10llljksd23rf.database.windows.net";
                builder.UserID = "rafa";
                builder.Password = "Omeghax9771";
                builder.InitialCatalog = "sigmasql";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    String sql = "select * from Producto";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                await stepContext.Context.SendActivityAsync("{0} {1}", reader.GetString(0), reader.GetString(1), cancellationToken: cancellationToken);
                                
                            }
                        }
                    }
                }
            }
            catch (SqlException )
            {
                await stepContext.Context.SendActivityAsync("Error", cancellationToken: cancellationToken);
                
            }
            
            
            
        }


        #region IntentLuis
        private async Task IntentVerOpciones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Error sql"), cancellationToken: cancellationToken);


            //await stepContext.Context.SendActivityAsync("Hola, que gusto verte", cancellationToken: cancellationToken);
            await MainOptionsCard.ToShow(stepContext, cancellationToken);
        }

        private void button1_Click(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "ein10llljksd23rf.database.windows.net";
                builder.UserID = "rafa";
                builder.Password = "Omeghax9771";
                builder.InitialCatalog = "sigmasql";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    String sql = "select * from Producto";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                await stepContext.Context.SendActivityAsync(MessageFactory.Text(reader.GetValue(0).ToString()), cancellationToken: cancellationToken);

                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Error sql"), cancellationToken: cancellationToken);

            }
            await stepContext.Context.SendActivityAsync("Hola, que gusto verte", cancellationToken: cancellationToken);
        }

        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No te preocupes, me gusta ayudar", cancellationToken: cancellationToken);
        }

        private async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Espero verte pronto", cancellationToken: cancellationToken);
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No entiendo lo que me dices", cancellationToken: cancellationToken);
        }
        #endregion

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
