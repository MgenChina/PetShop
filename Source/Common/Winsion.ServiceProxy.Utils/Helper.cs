using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Winsion.ServiceProxy.Utils
{
    internal static class Helper
    {
        public static void CloseChannel(ref IClientChannel proxy)
        {
            if (proxy != null)
            {
                switch (proxy.State)
                {
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        break;
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                    case CommunicationState.Opened:
                    case CommunicationState.Closing:
                        proxy.Close();
                        break;
                    default:
                        break;
                }
                proxy = null;
            }
        }

        public static void CloseChannelFactory<TChannel>(ref ChannelFactory<TChannel> channelFactory)
        {
            try
            {
                if (channelFactory != null)
                {
                    switch (channelFactory.State)
                    {
                        case CommunicationState.Faulted:
                            channelFactory.Abort();
                            break;
                        case CommunicationState.Created:
                        case CommunicationState.Opening:
                        case CommunicationState.Opened:
                        case CommunicationState.Closing:
                            channelFactory.Close();
                            break;
                        default:
                            break;
                    }
                    channelFactory = null;
                }
            }
            catch (System.Exception)
            {
                channelFactory.Abort();
                channelFactory = null;
            }
        }

        public static bool IsInvalidChannelFactory(IChannelFactory channelFactory)
        {
            bool isInvalid = true;
            if (channelFactory != null)
            {
                switch (channelFactory.State)
                {
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                    case CommunicationState.Opened:
                        isInvalid = false;
                        break;
                    default:
                        break;
                }
            }
            return isInvalid;
        }

    }
}
