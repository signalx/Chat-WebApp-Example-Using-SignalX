namespace SignalXChat.Web.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR.Messaging;
    using Newtonsoft.Json;
    using SignalXLib.Lib;

    public static class SignalXHubs
    {
        public static void Register()
        {
            SignalX.Instance.AuthenticationHandler(request => true);

            SignalX.Instance.Settings.ContinueClientExecutionWhenAnyServerOnClientReadyFails = false;
            SignalX.Instance.Server(
                "getMessageId",
                request =>
                {
                    request.RespondToSender(Guid.NewGuid().ToString());
                });
            SignalX.Instance.Server(
                "getUserMessages",
                request =>
                {
                   var userId= request.Message;
                    request.RespondToSender(MessageStore[userId]);
                });

            SignalX.Instance.Server(
                "sendMessage",
                request =>
                {
                    Messages message = JsonConvert.DeserializeObject<Messages>(JsonConvert.SerializeObject(request.Message));
                   Messages messages = JsonConvert.DeserializeObject<Messages>(JsonConvert.SerializeObject(request.Message));

                    MessageStore[message.receiver].Add(message);

                    request.RespondToAll("newMessage", message);
                   
                    
                    Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(
                        c =>
                        {
                            messages.sent = false;
                            MessageStore[messages.receiver].Add(messages);
                            SignalX.Instance.RespondToAll("newMessage", messages);
                        });
                });

            SignalX.Instance.ServerAuthorized(
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
                new User()
                {
                    active = false,
                    id= Id,
                    name = "SignalXUser " + Id,
                    date = "Dec " + (Id + 20),
                    description = "some description",
                    image = "https://ptetutorials.com/images/user-profile.png",
                  
                });
        }

        public static Dictionary<string, List<Messages>> MessageStore=new Dictionary<string, List<Messages>>()
        {
            {"sam",new List<Messages>
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
            {"bam",new List<Messages>
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
       
    }

    public class Messages
    {
        public string message;
        public bool sent;
        public string messageId;
        public string sender;
        public string receiver;

    }



    public class User
    {
        public string name;
        public string id;
        public bool active;
        public string date;
        public string image;
        public string description;
    }
    public class ChatList
    {
        [Obsolete]
        public string name;
        [Obsolete]
        public string date;
        [Obsolete]
        public string image;
        [Obsolete]
        public string description;


        public bool active;
        public string id;
    }
}