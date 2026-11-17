using System.Threading.Tasks;

namespace FADETools.IdeMessaging
{
    public interface IMessageHandler
    {
        public Task<MessageContent> HandleRequest(Peer peer, string id, MessageContent content, ILogger logger);
    }
}
