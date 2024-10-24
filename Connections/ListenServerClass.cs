using System.Net.WebSockets;
using System.Text;

namespace ComunicacaoWebSocketClient.Connections;

internal class ListenServerClass
{
    public static async Task ListenToServerAsync(ClientWebSocket client)
    {
        byte[] buffer = new byte[1024];
        try
        {

            while (client.State == WebSocketState.Open)
            {
                var messageReceived = new ArraySegment<byte>(buffer);
                var result = await client.ReceiveAsync(messageReceived, CancellationToken.None);

                string serverMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                //Show message from client to server
                Console.WriteLine(serverMessage);

            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Ocorreu um erro no ListenServerClass: {ex.Message}");
        }
    }
}
