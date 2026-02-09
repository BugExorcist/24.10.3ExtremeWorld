using SkillBridge.Message;

namespace GameServer.Models
{
    public class MapItem
    {
        public int MapItemId;
        public int ItemId;
        public int Count;
        public NVector3 Position;

        public NMapItem ToMessage()
        {
            return new NMapItem()
            {
                mapItemId = this.MapItemId,
                itemId = this.ItemId,
                Count = this.Count,
                Position = this.Position
            };
        }
    }
}
