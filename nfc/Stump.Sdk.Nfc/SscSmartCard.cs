using Stump.Sdk.Nfc.Types;
using Stump.Sdk.Nfc.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Stump.Sdk.Nfc
{
    public class SscSmartCard : INfcCard
    {
        private INfcReader reader { get; set; }

        public SscSmartCard(INfcReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Get SSC ID
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            var response = Read(0x04, 0x60, 0x00, 0x10);
            return Encoding.ASCII.GetString(response).Substring(0, 16).Trim();
        }
        public string GetUID()
        {
            var response = ReadUID();
            return string.Concat(response).Substring(0, 16).Trim();


        }

        public string GetUID1()
        {
            var response = Read(0x01, 0x60, 0x00, 0x10);
            return Encoding.UTF8.GetString(response).Trim();
        }

        /// <summary>
        /// Set SSC ID
        /// </summary>
        /// <param name="value"></param>
        public void SetId(string value)
        {
            Write(0x04, 0x60, 0x00, Encoding.ASCII.GetBytes(value));
        }

        public byte[] Read(byte block, byte keyType, byte keyNum, byte requestBytes)
        {
            var authBytes = new byte[] { 0xFF, 0x88, 0x00, block, keyType, keyNum };
            var cmdBytes = new byte[] { 0xFF, 0xB0, 0x00, block, requestBytes };

            reader.Send(authBytes);
            return reader.Send(cmdBytes);
        }
        public byte[] ReadUID()
        { 
            var cmdBytes = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
             
            return reader.Send(cmdBytes);
        }
        public byte[] Write(byte block, byte keyType, byte keyNum, byte[] data)
        {
            var authBytes = new byte[] { 0xFF, 0x88, 0x00, block, keyType, keyNum };
            reader.Send(authBytes);

            byte[] buffer = new byte[16];
            Array.Copy(data, buffer, data.Length);

            byte[] cmdBytes = new byte[21] {
                0xFF,0xD6,0x00,block,0x10,
                buffer[0],buffer[1],buffer[2],buffer[3],
                buffer[4],buffer[5],buffer[6],buffer[7],
                buffer[8],buffer[9],buffer[10],buffer[11],
                buffer[12],buffer[13],buffer[14],buffer[15]
            };

            return reader.Send(cmdBytes);
        }
    }
}
