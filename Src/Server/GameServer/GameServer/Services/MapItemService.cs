using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapItemService : Singleton<MapItemService>
    {
        public void Init()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapItemPickupRequest>(this.OnMapItemPickup);
        }

        private void OnMapItemPickup(NetConnection<NetSession> sender, MapItemPickupRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapItemPickup: Character:[{0}:{1}] MapItemId:{2}", character.Id, character.Name, request.mapItemId);

            if (character == null)
            {
                sender.Session.Response.mapItemPickup = new MapItemPickupResponse();
                sender.Session.Response.mapItemPickup.Result = Result.Failed;
                sender.Session.Response.mapItemPickup.Errormsg = "Character not found.";
                sender.SendResponse();
                return;
            }

            // Get Map
            // Assuming open world map for now. For instances, we might need instanceId from character or map manager logic.
            // MapManager[mapId] returns the first instance (0). 
            // If the game supports instances (like dungeons), we need character.Info.mapId and character.Info.instanceId (if available).
            // Looking at Character.cs (not fully read), but usually mapId is enough for open world.
            // Let's use MapManager.Instance[mapId] which is safe for open world.
            // TODO: Support instances if needed.
            Map map = MapManager.Instance[character.Info.mapId]; 
            if (map == null)
            {
                 sender.Session.Response.mapItemPickup = new MapItemPickupResponse();
                 sender.Session.Response.mapItemPickup.Result = Result.Failed;
                 sender.Session.Response.mapItemPickup.Errormsg = "Map not found.";
                 sender.SendResponse();
                 return;
            }

            // Get Item
            MapItem item = map.ItemManager.GetItem(request.mapItemId);
            if (item == null)
            {
                sender.Session.Response.mapItemPickup = new MapItemPickupResponse();
                sender.Session.Response.mapItemPickup.Result = Result.Failed;
                sender.Session.Response.mapItemPickup.Errormsg = "Item not found.";
                sender.SendResponse();
                return;
            }

            // Validate Distance (3 meters = 300 units)
            // Position is NVector3.
            // float dist = NVector3.Distance(character.Position, item.Position); // NVector3 doesn't have Distance
            long dx = character.Position.x - item.Position.X;
            long dy = character.Position.y - item.Position.Y;
            long dz = character.Position.z - item.Position.Z;
            long distSq = dx * dx + dy * dy + dz * dz;
            
            if (distSq > 300 * 300)
            {
                sender.Session.Response.mapItemPickup = new MapItemPickupResponse();
                sender.Session.Response.mapItemPickup.Result = Result.Failed;
                sender.Session.Response.mapItemPickup.Errormsg = "Item too far.";
                sender.SendResponse();
                return;
            }

            // Add to Bag
            if (character.ItemManager.AddItem(item.ItemId, item.Count))
            {
                // Remove from Map
                map.ItemManager.RemoveItem(item.MapItemId);

                // Send Response
                sender.Session.Response.mapItemPickup = new MapItemPickupResponse();
                sender.Session.Response.mapItemPickup.Result = Result.Success;
                sender.SendResponse();
            }
            else
            {
                sender.Session.Response.mapItemPickup = new MapItemPickupResponse();
                sender.Session.Response.mapItemPickup.Result = Result.Failed;
                sender.Session.Response.mapItemPickup.Errormsg = "Bag full.";
                sender.SendResponse();
            }
        }
    }
}
