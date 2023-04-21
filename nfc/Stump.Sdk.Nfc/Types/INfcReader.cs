using System.Threading.Tasks;

namespace Stump.Sdk.Nfc.Types
{
    public interface INfcReader
    {
        /// <summary>
        /// Connect to reader
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Send command to card
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        byte[] Send(byte[] request);

        /// <summary>
        /// Disconnect from reader
        /// </summary>
        void Disconnect();
    }
}
