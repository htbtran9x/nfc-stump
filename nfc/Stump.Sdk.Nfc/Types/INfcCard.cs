namespace Stump.Sdk.Nfc.Types
{
    public interface INfcCard
    {
        /// <summary>
        /// Read data from card
        /// </summary>
        /// <param name="block"></param>
        /// <param name="keyType"></param>
        /// <param name="keyNum"></param>
        /// <param name="requestBytes"></param>
        /// <returns></returns>
        public byte[] Read(byte block, byte keyType, byte keyNum, byte requestBytes);

        /// <summary>
        /// Write data to card
        /// </summary>
        /// <param name="block"></param>
        /// <param name="keyType"></param>
        /// <param name="keyNum"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Write(byte block, byte keyType, byte keyNum, byte[] data);
    }
}
