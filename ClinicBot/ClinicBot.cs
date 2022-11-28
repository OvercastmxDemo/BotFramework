// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ClinicBot
{
    public class ClinicBot : ActivityHandler
    {
        private readonly BotState _userstate;
        private readonly BotState _conversationState;
        private readonly Dialog.RootDialog _dialog;
        private string strProceso = "";
        private int countvar = 0;
        private string idProducto = "0";
        private int idSubProducto = 0;
        private int idDefecto = 0;
        

        public ClinicBot(UserState userState, ConversationState conversationState)
        {
            _userstate = userState;
            _conversationState = conversationState;
            

            

        }

        
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    

                    //await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!") );
                    await Cards(turnContext, countvar, cancellationToken);
                }
            }
        }

        /*public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _userstate.SaveChangesAsync(turnContext, false, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }*/

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            int countvar;
            Boolean banHola = false;
            string text = turnContext.Activity.Text.ToLower();
            /*
            if (text == "hola")
            {
                countvar = 0;
                Startup.C = 0;
                banHola = true;
                strProceso = "";
                Startup.ProcesoStr = strProceso;
                await Cards(turnContext, 0, cancellationToken);
            }
            else
            {
                banHola = false;
            }*/
            countvar = Startup.C;

            string[] strProducto;

            if (countvar == 1)
            {
                strProducto = text.Split('-');
                idProducto = strProducto[0].ToString();
                text = strProducto[1].ToString();
            }else if (countvar == 2)
            {
                strProducto = text.Split('-');
                idSubProducto = Int32.Parse(strProducto[0]);
                text = strProducto[1].ToString();
            }
            else if (countvar == 3)
            {
                strProducto = text.Split('-');
                idDefecto = Int32.Parse(strProducto[0]);
                text = strProducto[1].ToString();
                text = "Termino";
            }

            if (text == "Termino" || countvar == 4)
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

                        String sql = "select p.Nombre as 'Producto', sp.Nombre as 'Tipo', d.Nombre as 'Defecto' from SubProducto sp inner join Producto p on p.Id= sp.ProductoId inner join Defecto d on d.SubProductoId=sp.Id where d.Id = "+idDefecto.ToString()+";";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {   
                                    var ejemplo2 = "La prueba de esta consulta termina con la consulta de: \nProducto: "+reader.GetString(0)+" SubProducto:"+reader.GetString(1)+" Defecto:"+ reader.GetString(2);
                                    await turnContext.SendActivityAsync(ejemplo2);
                                    text = "adios";

                                }
                            }
                        }
                    }
                }
                catch (SqlException)
                {
                    await turnContext.SendActivityAsync("Error");

                }
            }

            if(text == "adios")
            {
                await turnContext.SendActivityAsync("Fue un gusto ayudarte");
                strProceso = "";
                Startup.ProcesoStr = strProceso;
                countvar = 0;
                Startup.C = countvar;
            }else if(countvar == 0 )
            {
                strProceso = "Defecto";
                Startup.ProcesoStr = strProceso;
                await turnContext.SendActivityAsync(strProceso);
                countvar++;
                Startup.C = countvar;

            }
            else if(countvar < 4)
            {
                strProceso = Startup.ProcesoStr;
                strProceso = strProceso + ">" + text;
                Startup.ProcesoStr = strProceso;
                await turnContext.SendActivityAsync(strProceso);
                countvar++;
                Startup.C = countvar;
            }
            else
            {
                countvar++;
                Startup.C = countvar;
            }



            var ejemplo = turnContext.Activity.Text;
            
            await Cards(turnContext, countvar, cancellationToken);
            
            /*await _dialog.RunAsync(turnContext,
                _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
                cancellationToken);*/
            /*
            object value = await _dialog.RunAsync(
                turnContext,
                _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
                cancellationToken
                );*/
        }

        protected async Task<ResourceResponse> Cards(ITurnContext TurnContext, int c, CancellationToken cancellationToken)
        {
            
           
            if (c == 0)
            {
                var reply = MessageFactory.Text("Hola usuario en que puedo ayudarte?");
                var optionsAttachments = new List<Attachment>();
                var cardDB = new HeroCard
                {
                    Title = "Selecciona una opcion",
                    Subtitle = "Opcion",
                    Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/imagen.png") },
                    Buttons = new List<CardAction>()
                    {
                        new CardAction(){Title = "Encontre un defecto", Value = "Defecto", Type = ActionTypes.ImBack},
                        new CardAction(){Title = "No", Value = "Adios", Type = ActionTypes.ImBack}
                    }
                };
                optionsAttachments.Add(cardDB.ToAttachment());
                reply.Attachments = optionsAttachments;
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;


                return await TurnContext.SendActivityAsync(reply as Activity);
            }else if (c == 1)
            {
                var reply = MessageFactory.Text("Seleciona un producto");
                var optionsAttachments = new List<Attachment>();
                


               
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

                        String sql = "select * from Producto;";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var cardDB = new HeroCard
                                    {
                                        Title = reader.GetString(1),
                                        Subtitle = "Producto",
                                        Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/imagen.png") },
                                        Buttons = new List<CardAction>()
                                        {
                                            new CardAction(){Title = "Seleccionar", Value = reader.GetValue(0).ToString()+"-"+reader.GetString(1), Type = ActionTypes.ImBack},
                                            
                                        }
                                    };
                                    optionsAttachments.Add(cardDB.ToAttachment());
                                    
                                    

                                }
                            }
                        }
                    }
                }
                catch (SqlException)
                {
                    await TurnContext.SendActivityAsync("Error");

                }

                reply.Attachments = optionsAttachments;
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return await TurnContext.SendActivityAsync(reply as Activity);

            }
            else if (c == 2)
            {
                var reply = MessageFactory.Text("Seleciona un subproducto");
                var optionsAttachments = new List<Attachment>();




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

                        String sql = "select * from SubProducto where ProductoId = "+idProducto+";";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var cardDB = new HeroCard
                                    {
                                        Title = reader.GetString(1),
                                        Subtitle = "SubProducto",
                                        Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/imagen.png") },
                                        Buttons = new List<CardAction>()
                                        {
                                            new CardAction(){Title = "Seleccionar", Value = reader.GetValue(0).ToString()+"-"+reader.GetString(1), Type = ActionTypes.ImBack},

                                        }
                                    };
                                    optionsAttachments.Add(cardDB.ToAttachment());



                                }
                            }
                        }
                    }
                }
                catch (SqlException)
                {
                    await TurnContext.SendActivityAsync("Error");

                }

                reply.Attachments = optionsAttachments;
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return await TurnContext.SendActivityAsync(reply as Activity);

            }
            else if (c == 3)
            {
                var reply = MessageFactory.Text("Seleciona un defecto");
                var optionsAttachments = new List<Attachment>();




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

                        String sql = "select * from Defecto where SubProductoId = "+idSubProducto.ToString()+";";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var cardDB = new HeroCard
                                    {
                                        Title = reader.GetString(1),
                                        Subtitle = "Defecto",
                                        Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/imagen.png") },
                                        Buttons = new List<CardAction>()
                                        {
                                            new CardAction(){Title = "Seleccionar", Value = reader.GetValue(0).ToString()+"-"+reader.GetString(1), Type = ActionTypes.ImBack},

                                        }
                                    };
                                    optionsAttachments.Add(cardDB.ToAttachment());



                                }
                            }
                        }
                    }
                }
                catch (SqlException)
                {
                    await TurnContext.SendActivityAsync("Error");

                }

                reply.Attachments = optionsAttachments;
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return await TurnContext.SendActivityAsync(reply as Activity);

            }

            else 
            {
                var reply = MessageFactory.Text(c.ToString());
                var optionsAttachments = new List<Attachment>();
                var cardDB = new HeroCard
                {
                    Title = "titulo",
                    Subtitle = "subtitulo",
                    Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/imagen.png") },
                    Buttons = new List<CardAction>()
                    {
                        new CardAction(){Title = "Encontre un defecto", Value = "Defecto", Type = ActionTypes.ImBack},
                        new CardAction(){Title = "No", Value = "Adios", Type = ActionTypes.ImBack}
                    }
                };
                optionsAttachments.Add(cardDB.ToAttachment());
                reply.Attachments = optionsAttachments;
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;


                return await TurnContext.SendActivityAsync(reply as Activity);
                // await TurnContext.SendActivitiesAsync(MessageFactory.Text(strProceso.ToString()),cancellationToken);
            }
            /*
            var cardDB = new HeroCard
            {
                Title = "titulo",
                Subtitle = "subtitulo",
                Images = new List<CardImage> { new CardImage("https://sigmabotstoreage.blob.core.windows.net/images/imagen.png") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Titulo buton", Value = "Valuebuton", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Titulo buton", Value = "Valuebuton", Type = ActionTypes.ImBack}
                }
            };
          

            var optionsAttachments = new List<Attachment>()
            {
                cardDB.ToAttachment()
            };

            optionsAttachments.Add(cardDB.ToAttachment());

            var reply = MessageFactory.Text("Te puedo ayudar en algo?");
            */
        }

        private void GetString(int v)
        {
            throw new NotImplementedException();
        }
    }
}
