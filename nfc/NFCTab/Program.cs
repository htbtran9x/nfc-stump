// See https://aka.ms/new-console-template for more information
using Stump.Sdk.Nfc;

Console.WriteLine("Hello, World!");

void ReadId(object sender, dynamic e)
{
    Console.WriteLine(e.Card.GetId());
}

ProcessNFC.ProcessNFCEvent += ReadId;

while (true)
{
    PcscNfcReader reader = new PcscNfcReader();
    
    await reader.ConnectAsync();
    
    SscSmartCard card = new SscSmartCard(reader);

    ProcessNFC.HandlerProcessNFC(card);

    Thread.Sleep(1000);

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