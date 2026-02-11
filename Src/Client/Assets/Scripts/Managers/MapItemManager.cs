using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using Services;
using Models;
using Common.Data;

namespace Managers
{
    // 地图掉落物
    public class MapItemManager : Singleton<MapItemManager>
    {
        private Dictionary<int, NMapItem> items = new Dictionary<int, NMapItem>();
        private Dictionary<int, GameObject> itemObjects = new Dictionary<int, GameObject>();

        public void Init()
        {
             MapItemService.Instance.OnMapItemSpawn += OnMapItemSpawn;
             MapItemService.Instance.OnMapItemRemove += OnMapItemRemove;
        }

        /// <summary>
        /// 掉落物生成
        /// </summary>
        private void OnMapItemSpawn(List<NMapItem> spawnedItems)
        {
            Debug.Log("刷新掉落物");
            foreach (var item in spawnedItems)
            {
                if (!items.ContainsKey(item.mapItemId))
                {
                    items.Add(item.mapItemId, item);
                    CreateItemObject(item);
                }
            }
        }

        /// <summary>
        /// 生成掉落物实体
        /// </summary>
        private void CreateItemObject(NMapItem item)
        {
            Debug.Log("创建掉落物");
            GameObject go = Resloader.Load<GameObject>("OtherModel/FallingObject/FallingObject");
            go = GameObject.Instantiate(go);
            Vector3 pos = GameObjectTool.LogicToWorld(item.Position) + Vector3.up * 0.5f;
            go.transform.position = pos;
            go.name = "MapItem_" + item.itemId;

            FallingObjects fallingObject = go.GetComponent<FallingObjects>();
            if (fallingObject != null)
            {
                ItemDefine itemDefine = DataManager.Instance.Items[item.itemId];
                if (itemDefine != null)
                {
                    fallingObject.SetSprite(itemDefine);
                }
            }
            
            itemObjects[item.mapItemId] = go;
        }

        private void OnMapItemRemove(int mapItemId)
        {
             if (items.ContainsKey(mapItemId))
            {
                items.Remove(mapItemId);
            }
            if (itemObjects.ContainsKey(mapItemId))
            {
                GameObject.Destroy(itemObjects[mapItemId]);
                itemObjects.Remove(mapItemId);
            }
        }
        
        public void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupNearestItem();
            }
        }
        
        private void PickupNearestItem()
        {
            if (User.Instance.CurrentCharacterObject == null) return;
            Vector3 playerPos = User.Instance.CurrentCharacterObject.transform.position;
            
            NMapItem nearest = null;
            float minDistSq = float.MaxValue;
            
            foreach (var item in items.Values)
            {
                float x = GameObjectTool.LogicToWorld(item.Position.X);
                float z = GameObjectTool.LogicToWorld(item.Position.Y);
                float y = GameObjectTool.LogicToWorld(item.Position.Z);
                
                Vector3 itemPos = new Vector3(x, y, z);
                
                float distSq = (playerPos - itemPos).sqrMagnitude;
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    nearest = item;
                }
            }
            
            if (nearest != null && minDistSq < 3.0f * 3.0f)
            {
                MapItemService.Instance.SendPickupRequest(nearest.mapItemId);
            }
        }
    }
}
