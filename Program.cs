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
        bool keepingloop = true;
        
        //Testando a conexão
        await webSocket.ConnectAsync(new Uri("ws://localhost:5139/"), CancellationToken.None);

        Console.WriteLine($"Connection successful");

        Task nome = SendName(webSocket);

        do
        {
            Task escolha = EscolhaDoUsuario(webSocket);

            Task listen = ListenToServerAsync(webSocket);

            while (webSocket.State == WebSocketState.Open)
            {
                string messageTest = Console.ReadLine()!;

                //Fecha a conexão se o tipo de mensagem for igual a mensagem de fechamento
                if (messageTest == "back")
                {
                    Console.Clear();
                    break;
                }
                Task dialogo = UserDialog(webSocket, messageTest);
            }

        } while (keepingloop && webSocket.State == WebSocketState.Open);

    }

}
catch (Exception ex) 
{
    Console.WriteLine($"Ocorreu um erro na classe Program: {ex.Message}");
}

async Task EscolhaDoUsuario(ClientWebSocket webSocket)
{

    //Get name
    Console.WriteLine("Menu de Opções: " +
        "\n1 - Conversar com pessoa" +
        "\n2 - Conversar com chat" +
        "\nEscolha uma das opções: ");
    string opcao = Console.ReadLine()!;
    byte[] opcaoEscolhida = Encoding.UTF8.GetBytes(opcao);

    //Send option of talk
    await webSocket.SendAsync(opcaoEscolhida, WebSocketMessageType.Text, true, CancellationToken.None);
}

async Task UserDialog(ClientWebSocket webSocket, string messageTest)
{
    if (messageTest == "exit")
    {
        Console.WriteLine("Connection closed");

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "CloseSent", CancellationToken.None);
    }
    else
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageTest);
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

}

async Task SendName(ClientWebSocket webSocket)
{
    //Get name
    Console.Write("Digite seu nome: ");
    string name = Console.ReadLine()!;
    byte[] dataName = Encoding.UTF8.GetBytes(name);

    //Send user name
    await webSocket.SendAsync(dataName, WebSocketMessageType.Text, true, CancellationToken.None);
}