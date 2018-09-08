namespace SignalXChat.Web.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SignalXLib.Lib;

    public static class SignalXHubs
    {
        public static void Register()
        {
            SignalX.Instance.AuthenticationHandler(request => true);

            SignalX.Instance.Server(
                "sendMessage",
                request =>
                {
                    request.RespondToAll("newMessage", request.Message);

                    Messages message = JsonConvert.DeserializeObject<Messages>(JsonConvert.SerializeObject(request.Message));
                    message.sent = false;
                    Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(
                        c => { SignalX.Instance.RespondToAll("newMessage", message); });
                });

            SignalX.Instance.ServerAuthorized(
                "getChats",
                (request, state) =>
                {
                    var list = new List<ChatList>();
                    int Id = 0;
                    NewMethod(list, Id);
                    request.RespondToSender(list);

                    Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(
                        c =>
                        {
                            NewMethod(list, Id);
                            SignalX.Instance.RespondToAll("receiveNewConversation", list);
                        });
                });
        }

        static void NewMethod(List<ChatList> list, int Id)
        {
            list.Add(
                new ChatList
                {
                    active = false,
                    name = "SignalXUser " + Id,
                    date = "Dec " + (Id + 20),
                    lastMessage = "Test, which is a new approach to have all solutions astrology under one roof.",
                    image = "https://ptetutorials.com/images/user-profile.png",
                    messages = new List<Messages>
                    {
                        createMessage(true),
                        createMessage(),
                        createMessage(true),
                        createMessage()
                    }
                });
        }

        public static Messages createMessage(bool sent = false)
        {
            return new Messages
            {
                sent = sent,
                message = "yo so we are " + Guid.NewGuid()
            };
        }
    }

    public class Messages
    {
        public string message;
        public bool sent;
    }

    public class ChatList
    {
        public bool active;
        public string date;
        public string image;
        public string lastMessage;
        public object messages;
        public string name;
    }
}