// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

NFCReader nfc = new NFCReader();



nfc.CardInserted += new NFCReader.CardEventHandler(Read);

void Read() {

    Console.WriteLine(nfc.GetCardUID());
}

while(true) {

    nfc.Connect();
    Read();
    Thread.Sleep(100);

    nfc.Disconnect();

 }

