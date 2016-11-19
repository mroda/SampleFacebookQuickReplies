using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

namespace SampleFacebookQuickReplies
{
    [Serializable]
    public class SampleDialog : IDialog<object>
    {
        const string channelIdFacebook = "facebook";
        const string payloadBoy = "DEFINED_PAYLOAD_FOR_PICKING_6TO12";
        const string payloadTeen = "DEFINED_PAYLOAD_FOR_PICKING_12TO19";
        const string payloadYoung = "DEFINED_PAYLOAD_FOR_PICKING_20TO35";
        const string payloadOld = "DEFINED_PAYLOAD_FOR_PICKING_35ONWARDS";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            await context.PostAsync("Hola soy un Robot, no puedo creer que estemos chateando!");

            var reply = context.MakeMessage();
            reply.Text = "Dime en que rango de edad estas?";

            if (message.ChannelId.Equals(SampleDialog.channelIdFacebook, StringComparison.InvariantCultureIgnoreCase))
            {
                var channelData = JObject.FromObject(new
                {
                    quick_replies = new dynamic[]
                    {
                        new
                        {
                            content_type = "text",
                            title = "6 a 12",
                            payload = SampleDialog.payloadBoy,
                        },
                        new
                        {
                            content_type = "text",
                            title = "13 a 19",
                            payload = SampleDialog.payloadTeen,
                        },
                        new
                        {
                            content_type = "text",
                            title = "20 a 35",
                            payload = SampleDialog.payloadYoung,
                        },
                         new
                        {
                            content_type = "text",
                            title = "+35",
                            payload = SampleDialog.payloadOld,
                        }
                    }
                });

                reply.ChannelData = channelData;

            }

            await context.PostAsync(reply);
            context.Wait(this.OnOptionPicked);
        }

        private async Task OnOptionPicked(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var textMessage = string.Empty;

            if (message.ChannelId.Equals(SampleDialog.channelIdFacebook, StringComparison.InvariantCultureIgnoreCase))
            {
                var quickReplyResponse = message.ChannelData.message.quick_reply;

                if (quickReplyResponse != null)
                {
                    textMessage = this.GetMessageFromPayload(Convert.ToString(quickReplyResponse.payload));
                }
                else
                {
                    textMessage += "No seleccionaste ninguna opcion.";
                }
            }

            await context.PostAsync(textMessage);
            context.Wait(this.MessageReceivedAsync);
        }

        private string GetMessageFromPayload(string payload)
        {
            switch (payload)
            {
                case SampleDialog.payloadBoy:
                    return "Eres un nino, vete a la escuela";
                case SampleDialog.payloadTeen:
                    return "Eres un adolescente, estudia y portate bien!";
                case SampleDialog.payloadYoung:
                    return "Eres Joven, como yo :)";
                default:
                    return "Eres un viejito jojo";
            }
        }
    }
}