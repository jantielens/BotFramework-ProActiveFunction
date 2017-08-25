using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;

/* Sample body json
{
    "messagetext" : "message text to send",
    "fromid" : "",
    "fromname" : "",
    "toid" : "",
    "toname" : "",
    "serviceurl" : "",
    "appid" : "",
    "apppassword" : ""
}
*/


namespace BotFrameworkProactiveMessage
{
    public static class SendToConversation
    {
        [FunctionName("SendToConversation")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("SendToConversation C# HTTP trigger function processed a request.");

            // parse query parameters
            string messageText = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "message", true) == 0)
                .Value;
            string toName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "toname", true) == 0)
                .Value;
            string toId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "toid", true) == 0)
                .Value;
            string fromId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "fromid", true) == 0)
                .Value;
            string fromName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "fromname", true) == 0)
                .Value;
            string serviceUrl = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "serviceurl", true) == 0)
                .Value;
            string appId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "appid", true) == 0)
                .Value;
            string appPassword = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "apppassword", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            // Set name to query string or body data
            messageText = messageText ?? data?.messagetext;
            toName = toName ?? data?.toname;
            toId = toId ?? data?.toid;
            fromId = fromId ?? data?.fromid;
            fromName = fromName ?? data?.fromname;
            serviceUrl = serviceUrl ?? data?.serviceurl;
            appId = appId ?? data?.appid;
            appPassword = appPassword ?? data?.apppassword;

            try
            {
                var userAccount = new ChannelAccount(toId, toName);
                var botAccount = new ChannelAccount(fromId, fromName);

                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
                MicrosoftAppCredentials creds = new MicrosoftAppCredentials(appId, appPassword);
                var connector = new ConnectorClient(new Uri(serviceUrl), creds);

                IMessageActivity message = Activity.CreateMessageActivity();
                var conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;

                message.From = botAccount;
                message.Recipient = userAccount;
                message.Conversation = new ConversationAccount(id: conversationId);
                message.Text = messageText;
                message.Locale = "en-Us";
                await connector.Conversations.SendToConversationAsync((Activity)message);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
