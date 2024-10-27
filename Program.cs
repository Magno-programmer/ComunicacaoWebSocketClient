using System;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
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

        string nome = SendName();

        byte[] dataName = Encoding.UTF8.GetBytes(nome);

        //Send user name
        await webSocket.SendAsync(dataName, WebSocketMessageType.Text, true, CancellationToken.None);

        do
        {
            Console.Clear();
            EscolhaDoUsuario(nome);

            string escolha = Console.ReadLine()!;

            if (!escolha.All(char.IsDigit) || escolha == "")
            {
                await Contagem(3, "Invalid Option", nome);
                Console.Clear();
                continue;
            
            }else if (int.Parse(escolha) >= 3)
            {
                string texto;
                if(int.Parse(escolha) == 3)
                    texto = "Em breve..";
                else
                    texto = "Invalid Option";

                await Contagem(3, texto, nome);
                Console.Clear();
                continue;
            }

            byte[] opcaoEscolhida = Encoding.UTF8.GetBytes(escolha);

            //Send option of talk
            await webSocket.SendAsync(opcaoEscolhida, WebSocketMessageType.Text, true, CancellationToken.None);

            Task listen = ListenToServerAsync(webSocket);

            while (webSocket.State == WebSocketState.Open)
            {
                string messageTest = Console.ReadLine()!;

                //Fecha a conexão se o tipo de mensagem for igual a mensagem de fechamento
                if (messageTest == "back")
                {
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


async Task Contagem(int seg, string mensagem, string nome)
{
    for (int i = seg; i > 0; i--)
    {
        Console.Clear();
        EscolhaDoUsuario(nome);
        Console.WriteLine($"{mensagem}. Retornando ao menu em: {i} second(s)...");
        await Task.Delay(1000);  // Espera 1 segundo para cada contagem
    }
}
void EscolhaDoUsuario(string nome)
{ 
    Console.WriteLine("Menu de Opções: " +
        "\n1 - Conversar no grupo" +
        "\n2 - Conversar com chat" +
        "\n3 - Conversar com um amigo(Em breve...)" +
        $"\nBem vindo {nome}. Escolha uma das opções: ");

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

string SendName()
{
    //Get name
    Console.Write("Digite seu nome: ");
    string name = Console.ReadLine()!;
    return name;
}