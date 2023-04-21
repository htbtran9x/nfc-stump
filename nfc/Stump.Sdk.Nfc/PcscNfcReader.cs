using PCSC;
using Stump.Sdk.Nfc.Types;
using System;
using System.Threading.Tasks;

namespace Stump.Sdk.Nfc
{
    public class PcscNfcReader : IDisposable, INfcReader
    {
        protected ISCardContext Context { get; set; }
        protected ICardReader Reader { get; set; }

        public bool Connected { get; set; }

        public async Task ConnectAsync()
        {
            if (Context == null)
            {
                Context = ContextFactory.Instance.Establish(SCardScope.System);
            }

            try
            {
                Reader = Context.ConnectReader(Context.GetReaders()[0], SCardShareMode.Shared, SCardProtocol.Any);
                Connected = true;
            }
            catch (PCSC.Exceptions.RemovedCardException)
            {
                await Task.Delay(100);
                await this.ConnectAsync();
            }
        }

        public void Disconnect()
        {
            Reader.Dispose();
            Reader.Dispose();
            Connected = false;
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        public byte[] Send(byte[] request)
        {
            byte[] recivedBuffer = new byte[20];
            Reader.Transmit(request, recivedBuffer);

            return recivedBuffer;
        }
        public async Task<byte[]> SendAsync(byte[] request)
        {
            byte[] recivedBuffer = new byte[20];
            Reader.Transmit(request, recivedBuffer);

            return  recivedBuffer;
        }
    }
}
