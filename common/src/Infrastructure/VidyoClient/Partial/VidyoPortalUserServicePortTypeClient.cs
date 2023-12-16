using System;
using System.ServiceModel;

// ReSharper disable once CheckNamespace
namespace VidyoService
{
    public partial class VidyoPortalUserServicePortTypeClient
    {
        public VidyoPortalUserServicePortTypeClient(string endpointUrl, string username, string password, TimeSpan timeout) :
            base(GetBindingForEndpoint(timeout), GetEndpointAddress(endpointUrl))
        {
            this.ChannelFactory.Credentials.UserName.UserName = username;
            this.ChannelFactory.Credentials.UserName.Password = password;
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(TimeSpan timeout)
        {
            var httpsBinding = new BasicHttpsBinding();
            httpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            httpsBinding.Security.Mode = BasicHttpsSecurityMode.Transport;

            var integerMaxValue = int.MaxValue;
            httpsBinding.MaxBufferSize = integerMaxValue;
            httpsBinding.MaxReceivedMessageSize = integerMaxValue;
            httpsBinding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            httpsBinding.AllowCookies = true;

            httpsBinding.ReceiveTimeout = timeout;
            httpsBinding.SendTimeout = timeout;
            httpsBinding.OpenTimeout = timeout;
            httpsBinding.CloseTimeout = timeout;

            return httpsBinding;
        }

        private static EndpointAddress GetEndpointAddress(string endpointUrl)
        {
            if (!endpointUrl.StartsWith("https://"))
            {
                throw new UriFormatException("The endpoint URL must start with https://.");
            }
            return new EndpointAddress(endpointUrl);
        }
    }
}
