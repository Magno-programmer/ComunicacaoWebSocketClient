using System;
using System.Net.WebSockets;
using System.Text;
using static ComunicacaoWebSocketClient.Connections.ListenServerClass;

try
{

    Console.WriteLine($"Connecting with web socket...");
    await Task.Delay(1000);


    //Criando o WebSocket do cliente
    using (ClientWebSocket webSocket = new ClientWebSocket())
    {
        byte[] buffer = new byte[1024];
        //Testando a conexão
        Task connection = webSocket.ConnectAsync(new Uri("ws://localhost:5139/"), CancellationToken.None);
        await connection;

        Console.WriteLine($"Connection successful");

        //Get name
        Console.Write("Digite seu nome: ");
        string name = Console.ReadLine()!;
        byte[] dataName = Encoding.UTF8.GetBytes(name);

        //Send user name
        await webSocket.SendAsync(dataName, WebSocketMessageType.Text, true, CancellationToken.None);

        //Receive Id of client
        var resultid = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string receivedId = Encoding.UTF8.GetString(buffer, 0, resultid.Count);

        Console.WriteLine($"You are connected with Id: {receivedId}");

        Task Listen = ListenToServerAsync(webSocket);


        while (webSocket.State == WebSocketState.Open)
        {

            string messageTest = Console.ReadLine()!;

            //Fecha a conexão se o tipo de mensagem for igual a mensagem de fechamento
            if (messageTest == "exit")
            {
                Console.WriteLine("Connection closed");
                break;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(messageTest);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        }

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

    }

}
catch (Exception ex) 
{
    Console.WriteLine($"Ocorreu um erro na classe Program: {ex.Message}");
}