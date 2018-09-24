namespace SignalXChat.Web.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SignalXLib.Lib;

    public static class SignalXHubs
    {
        public static Dictionary<string, List<Messages>> MessageStore = new Dictionary<string, List<Messages>>
        {
            {
                "sam", new List<Messages>
                {
                    new Messages
                    {
                        sent = true,
                        message = "yo so we are " + Guid.NewGuid(),
                        messageId = Guid.NewGuid().ToString(),
                        sender = "",
                        receiver = ""
                    },
                    new Messages
                    {
                        sent = false,
                        message = "yo so we are " + Guid.NewGuid(),
                        messageId = Guid.NewGuid().ToString(),
                        sender = "",
                        receiver = ""
                    }
                }
            },
            {
                "bam", new List<Messages>
                {
                    new Messages
                    {
                        sent = true,
                        message = "yo so we are " + Guid.NewGuid(),
                        messageId = Guid.NewGuid().ToString(),
                        sender = "",
                        receiver = ""
                    },
                    new Messages
                    {
                        sent = false,
                        message = "yo so we are " + Guid.NewGuid(),
                        messageId = Guid.NewGuid().ToString(),
                        sender = "",
                        receiver = ""
                    }
                }
            }
        };

        public static async Task Register()
        {
            SignalX.Instance.AuthenticationHandler( request => Task.FromResult(true));

            SignalX.Instance.Settings.ContinueClientExecutionWhenAnyServerOnClientReadyFails = false;
            SignalX.Instance.Server(
                "getMessageId",
                request => { request.RespondToSender(Guid.NewGuid().ToString()); });
            SignalX.Instance.Server(
                "getUserMessages",
                request =>
                {
                    dynamic userId = request.MessageAs<string>();
                    request.RespondToSender(MessageStore[userId]);
                });
            SignalX.Instance.OnException(
                (e) =>
                {

                });
            SignalX.Instance.ServerAsync(
                "sendMessage",
               async (request,t) =>
                {
                    Messages message = request.MessageAs<Messages>();
                    Messages messages = request.MessageAs<Messages>(); 

                    MessageStore[message.receiver].Add(message);

                    request.RespondToAll("newMessage", message);

                   await  Task.Delay(TimeSpan.FromSeconds(3));

                    messages.sent = false;
                    MessageStore[messages.receiver].Add(messages);
                    SignalX.Instance.RespondToAll("newMessage", messages);
                });

            SignalX.Instance.Server(ServerType.Authorized,
                "getChats",
                (request, state) =>
                {
                    var list = new List<User>();
                    NewMethod(list, "bam");
                    NewMethod(list, "sam");
                    request.RespondToSender(list);
                });
        }

        static void NewMethod(List<User> list, string Id)
        {
            list.Add(
                new User
                {
                    active = false,
                    id = Id,
                    name = "SignalXUser " + Id,
                    date = "Dec " + (Id + 20),
                    description = "some description",
                    image = "https://ptetutorials.com/images/user-profile.png"
                });
        }
    }

    public class Messages
    {
        public string message;
        public string messageId;
        public string receiver;
        public string sender;
        public bool sent;
    }

    public class User
    {
        public bool active;
        public string date;
        public string description;
        public string id;
        public string image;
        public string name;
    }

    public class ChatList
    {
        public bool active;

        [Obsolete]
        public string date;

        [Obsolete]
        public string description;

        public string id;

        [Obsolete]
        public string image;

        [Obsolete]
        public string name;
    }
}