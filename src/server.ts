import { WebSocketServer, WebSocket } from "ws";

const wss = new WebSocketServer({ port: 8080 });

//in-memory storage (replace with database later)
const rooms = new Map<string, any>();
const clients = new Map<WebSocket, { playerId: string; roomId: string }>();

wss.on("connection", (ws) => {
  console.log("Client connected");

  ws.on("message", (data) => {
    const message = JSON.parse(data.toString());

    if (message.type === "JOIN_ROOM") {
      const { roomId, playerName } = message;
      const playerId = crypto.randomUUID();

      //create room if doesn't exist
      if (!rooms.has(roomId)) {
        rooms.set(roomId, {
          id: roomId,
          players: [],
          phase: "lobby",
        });
      }

      //add player
      const room = rooms.get(roomId);
      room.players.push({
        id: playerId,
        name: playerName,
        connected: true,
      });

      //track this connection
      clients.set(ws, { playerId, roomId });

      //broadcast to everyone in room
      broadcast(roomId, {
        type: "ROOM_UPDATE",
        room: room,
      });
    }
  });

  function broadcast(roomId: string, message: any) {
    clients.forEach((clientData, clientWs) => {
      if (
        clientData.roomId === roomId &&
        clientWs.readyState === WebSocket.OPEN
      ) {
        clientWs.send(JSON.stringify(message));
      }
    });
  }

  ws.on("close", () => {
    console.log("Client disconnected");

    const clientData = clients.get(ws);

    if (clientData) {
      const room = rooms.get(clientData.roomId);
      if (room) {
        const disconnectedPlayer = room.players.find(p => p.id === clientData.playerId);
        const playerName = disconnectedPlayer ? disconnectedPlayer.name : "Unknown Player";

        room.players = room.players.filter((p) => p.id !== clientData.playerId);
        //Broadcast update
        broadcast(clientData.roomId, {
          type: "ROOM_UPDATE",
          room: rooms,
          message: playerName + " disconnected"
        });
      }

      //remove from clients map
      clients.delete(ws);
    }
  });
});
