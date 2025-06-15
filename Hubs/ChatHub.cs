using Microsoft.AspNetCore.SignalR;
using System;                         // 'Exception' sınıfını tanımak için GEREKLİ!
using System.Collections.Generic;     // 'Dictionary' sınıfını tanımak için GEREKLİ!
using System.Linq;                    // 'FirstOrDefault' gibi komutlar için GEREKLİ!
using System.Threading.Tasks;         // 'Task' gibi asenkron işlemleri tanımak için GEREKLİ!

// Senin projenin namespace'i farklı olabilir, bu satırı kendininkine göre değiştirmene gerek yok, orijinal haliyle kalsın.
namespace TelsizSohbetUygulamasi.Hubs 
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Rooms = new Dictionary<string, Dictionary<string, string>>();

        public async Task JoinRoom(string roomName, string userName)
        {
            string connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, roomName);

            if (!Rooms.ContainsKey(roomName))
            {
                Rooms[roomName] = new Dictionary<string, string>();
            }
            Rooms[roomName][connectionId] = userName;

            await Clients.Group(roomName).SendAsync("ReceiveMessage", "Sistem", $"{userName} odaya katıldı.");
            await SendUsersConnected(roomName);
        }

        public async Task SendMessageToGroup(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        }

        // Bu metot, Hub sınıfından miras alındığı için 'override' anahtar kelimesini kullanır.
        // Bütün 'using' direktifleri eklendiği için artık hata vermeyecek.
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            var roomKVP = Rooms.FirstOrDefault(r => r.Value.ContainsKey(connectionId));

            if (roomKVP.Key != null)
            {
                string roomName = roomKVP.Key;
                string userName = Rooms[roomName][connectionId];

                Rooms[roomName].Remove(connectionId);

                if (Rooms[roomName].Count == 0)
                {
                    Rooms.Remove(roomName);
                }

                await Clients.Group(roomName).SendAsync("ReceiveMessage", "Sistem", $"{userName} odadan ayrıldı.");
                await SendUsersConnected(roomName);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersConnected(string roomName)
        {
            if (Rooms.ContainsKey(roomName))
            {
                var users = Rooms[roomName].Values.ToList();
                return Clients.Group(roomName).SendAsync("UsersInRoom", users);
            }
            return Task.CompletedTask;
        }
    }
}