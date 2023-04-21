using Stump.Sdk.Nfc.Types;
using Stump.Sdk.Nfc.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> GetIdAsync()
        {
            var response = await ReadAsync(0x04, 0x60, 0x00, 0x10);
            return Encoding.ASCII.GetString(response).Substring(0, 16).Trim();
        }

        public async Task<string> GetUIDAsync()
        {
            var response = await ReadUIDAsync();
            return string.Concat(response).Substring(0, 16).Trim();


        }
        public string GetName()
        {
            var s5Response = Read(0x05, 0x60, 0x00, 0x10);
            var s6Response = Read(0x06, 0x60, 0x00, 0x10);

            var response = new byte[32];
            Array.Copy(s5Response, response, 16);
            Array.Copy(s6Response, 0, response, 16, 16);

            return Encoding.ASCII.GetString(response).Split('0')[0].Trim();
        }
        public string GetHash()
        {
            var s5Response = Read(0x05, 0x60, 0x00, 0x10);
            var s6Response = Read(0x06, 0x60, 0x00, 0x10);
            var s8Response = Read(0x08, 0x60, 0x00, 0x10);

            var response = new byte[60];
            Array.Copy(s5Response, response, 16);
            Array.Copy(s6Response, 0, response, 16, 16);
            Array.Copy(s8Response, 0, response, 32, 16);

            return Encoding.ASCII.GetString(response).Split("0000")[0].Trim();
        }
        public async Task<string> GetHashAsync()
        {
            var s5Response = ReadAsync(0x05, 0x60, 0x00, 0x10);
            var s6Response = ReadAsync(0x06, 0x60, 0x00, 0x10);
            var s8Response = ReadAsync(0x08, 0x60, 0x00, 0x10);

            await  Task.WhenAll(s5Response, s6Response, s8Response);
            var response = new byte[60];
            Array.Copy(s5Response.Result, response, 16);
            Array.Copy(s6Response.Result, 0, response, 16, 16);
            Array.Copy(s8Response.Result, 0, response, 32, 16);

            return Encoding.ASCII.GetString(response).Split("0000")[0].Trim();
        }
        /// <summary>
        /// Set owner name
        /// </summary>
        /// <param name="value">New value</param>
        public void SetName(string value)
        {
            if (value.Length > 32)
            {
                throw new InvalidOperationException("Owner name mus be a 32-char string");
            }

            Write(0x05, 0x60, 0x00, Encoding.ASCII.GetBytes(value.Length < 16 ? value : value.Substring(0, 16)));
            if (value.Length > 16)
            {
                Write(0x06, 0x60, 0x00, Encoding.ASCII.GetBytes(value.Substring(16, value.Length - 16)));
            }
            else
            {
                Write(0x06, 0x60, 0x00, Encoding.ASCII.GetBytes("                "));
            }
        }

        public void SetValidId(string value)
        {
            if (value.Length > 48)
            {
                throw new InvalidOperationException("Owner name mus be a 48-char string");
            } 
            Write(0x05, 0x60, 0x00, Encoding.ASCII.GetBytes(value.Substring(0, 16)));
            Write(0x06, 0x60, 0x00, Encoding.ASCII.GetBytes(value.Substring(16, 16)));
            Write(0x08, 0x60, 0x00, Encoding.ASCII.GetBytes(value.Substring(32, 16)));
            
        }
        /// <summary>
        /// Get card balance
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetBalance(int key)
        {
            // SECTOR: 8

            var response = Read(0x08, 0x60, 0x00, 0x10).Take(4).ToArray();
            return BitConverter.ToInt32(response.XOR(BitConverter.GetBytes(key)), 0);
        }

        /// <summary>
        /// Set SSC ID
        /// </summary>
        /// <param name="value"></param>
        public void SetId(string value)
        {
            Write(0x04, 0x60, 0x00, Encoding.ASCII.GetBytes(value));
        }

        public  byte[] Read(byte block, byte keyType, byte keyNum, byte requestBytes)
        {
            var authBytes = new byte[] { 0xFF, 0x88, 0x00, block, keyType, keyNum };
            var cmdBytes = new byte[] { 0xFF, 0xB0, 0x00, block, requestBytes };

            reader.Send(authBytes);
            return reader.Send(cmdBytes);
        }

        public async Task<byte[]> ReadAsync(byte block, byte keyType, byte keyNum, byte requestBytes)
        {
            var authBytes = new byte[] { 0xFF, 0x88, 0x00, block, keyType, keyNum };
            var cmdBytes = new byte[] { 0xFF, 0xB0, 0x00, block, requestBytes };

            reader.Send(authBytes);
            return await reader.SendAsync(cmdBytes);
        }
        public byte[] ReadUID()
        { 
            var cmdBytes = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
             
            return reader.Send(cmdBytes);
        }
        public async Task<byte[]> ReadUIDAsync()
        {
            var cmdBytes = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };

            return await reader.SendAsync(cmdBytes);
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
