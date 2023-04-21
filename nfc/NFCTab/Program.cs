// See https://aka.ms/new-console-template for more information
using Stump.Sdk.Nfc;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hello, World!");

void ReadId(object sender, dynamic e)
{
    var a = e as CardArgs;
    Console.Write("UUID: ");
    Console.WriteLine(a.Card.GetUID());
    Console.Write("SSCID: ");
    Console.WriteLine(a.Card.GetId());
    Console.Write("Name: ");
    Console.WriteLine(a.Card.GetName());
    Console.WriteLine("-----------------------------------------------------------");

}

void ReadHash(object sender, dynamic e)
{
    var a = e as CardArgs;
    Console.Write("UUID: ");
    Console.WriteLine(a.Card.GetUID());
    Console.Write("SSCID: ");
    Console.WriteLine(a.Card.GetId());
    Console.Write("HashValue: ");
    Console.WriteLine(a.Card.GetHash());
    Console.WriteLine("-----------------------------------------------------------");

}

async void ReadHashAsync(object sender, dynamic e)
{
    var a = e as CardArgs;
    var uid = a.Card.GetUIDAsync();
    var id = a.Card.GetIdAsync();
    var hash = a.Card.GetHashAsync();

    await Task.WhenAll(uid, id, hash);
    Console.Write("UUID: ");
    Console.WriteLine(uid.Result);
    Console.Write("SSCID: ");
    Console.WriteLine(id.Result);
    Console.Write("HashValue: ");
    Console.WriteLine(hash.Result);
    Console.WriteLine("-----------------------------------------------------------");

}
void Write(object sender, dynamic e)
{
    //e.Card.SetId("1202124217000561");
    //e.Card.SetName("HUYNH THAI BAO TRAN");
    //Console.Write("SSCID: ");
    //Console.WriteLine(e.Card.GetId());
    //Console.Write("Name: ");
    //Console.WriteLine(e.Card.GetName());
    var a = e as CardArgs;
    var uuid = a.Card.GetUID();
    var sscid = a.Card.GetId();
    string valueToHash = uuid + sscid;
    string key = "0123456789";
    // hash 94784db1aad5fe93d2af31c80c4d8b7bbe072f7b40b1488a
    byte[] keyBytes = Encoding.ASCII.GetBytes(key);
    byte[] valueBytes = Encoding.ASCII.GetBytes(valueToHash);

    using (var hmac = new HMACSHA256(keyBytes))
    {
        byte[] hash = hmac.ComputeHash(valueBytes);
        string hashString = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower().Substring(0, 48);
        a.Card.SetValidId(hashString);
      //   Console.WriteLine(hashString);
        //Console.WriteLine(hashString.Length);
        Console.WriteLine(a.Card.GetHash());
    }
}
//ProcessNFC.ProcessNFCEvent += ReadId;
//ProcessNFC.ProcessNFCEvent += Write;
 ProcessNFC.ProcessNFCEvent += ReadHashAsync;
PcscNfcReader reader = new PcscNfcReader();

while (true)
{ 
    await reader.ConnectAsync();

    SscSmartCard card = new SscSmartCard(reader);

    ProcessNFC.HandlerProcessNFC(card);
    Thread.Sleep(500);
    reader.Disconnect();
   
       
}
public class ProcessNFC
{
    public static EventHandler ProcessNFCEvent;
    public static void HandlerProcessNFC(SscSmartCard card)
    {
        ProcessNFCEvent.Invoke(null, new CardArgs
        {
            Card = card
        });
    }
}

public class CardArgs : EventArgs
{
    public SscSmartCard Card { get; set; }
}