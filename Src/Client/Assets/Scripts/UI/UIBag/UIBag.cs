using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public TMP_Text title;

    public TMP_Text money;

    public Transform[] pages;

    public GameObject bagItems;//UIBagItem预制体

    List<Image> slots;//ÿ�����ӵ�Image
    List<List<Image>> pageSlots;
    private UIItemPopUp itemPopUp;

    private void Start()
    {
        //����UI����
        User.Instance.OnUpdataGold += UpdataGold;
        BagManager.Instance.OnUpdateItems += UpdateItems;

        if (slots == null)
        {
            slots = new List<Image>();
            pageSlots = new List<List<Image>>();
            for (int page = 0; page < pages.Length; page++)
            {
                List<Image> currentPageSlots = new List<Image>(this.pages[page].GetComponentsInChildren<Image>(true));
                pageSlots.Add(currentPageSlots);
                slots.AddRange(currentPageSlots);
            }
        }
        StartCoroutine(InitBags());
    }

    private void OnDestroy()
    {
        CloseItemPopUp();
        User.Instance.OnUpdataGold -= UpdataGold;
        BagManager.Instance.OnUpdateItems -= UpdateItems;
    }

        IEnumerator InitBags()
    {
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            var slot = GetSlotByBagIndex(i);
            if (slot == null)
                continue;

            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItems, slot.transform);
                UIIconItem ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString(), def.QualityColor);
            }
        }
        for (int i = BagManager.Instance.Unlocked; i < BagManager.Instance.Items.Length; i++)
        {
            var slot = GetSlotByBagIndex(i);
            if (slot != null)
                slot.color = Color.gray;
        }
        yield return null;
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
    }

    private void UpdataGold()
    {
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }

    private void UpdateItems()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            var slot = GetSlotByBagIndex(i);
            if (slot == null)
                continue;

            UIIconItem ui = slot.transform.GetComponentInChildren<UIIconItem>();
            if (item.ItemId <= 0 && ui != null)
            {
                foreach (Transform chile in slot.transform)
                {
                    Destroy(chile);
                }
            }
            else if (item.ItemId > 0)
            {
                if (ui != null)
                {
                    var def = ItemManager.Instance.Items[item.ItemId].Define;
                    ui.SetMainIcon(def.Icon, item.Count.ToString(), def.QualityColor);
                }
                else
                {
                    GameObject go = Instantiate(bagItems, slot.transform);
                    ui = go.GetComponent<UIIconItem>();
                    var def = ItemManager.Instance.Items[item.ItemId].Define;
                    ui.SetMainIcon(def.Icon, item.Count.ToString(), def.QualityColor);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))// 右键点击
        {
            int slotIdx = CheckClickSlotIdx();
            if (slotIdx >= 0 && slotIdx < BagManager.Instance.Items.Length)
            {
                var bagItem = BagManager.Instance.Items[slotIdx];
                if (bagItem.ItemId > 0 && bagItem.Count > 0)
                {
                    Item item;
                    bool canPopUp = ItemManager.Instance.Items.TryGetValue(bagItem.ItemId, out item)
                        && item != null
                        && item.Define != null
                        && item.Define.CanUse;

                    if (canPopUp)
                    {
                        itemPopUp = UIManager.Instance.Show<UIItemPopUp>();
                        if (itemPopUp != null)
                        {
                            itemPopUp.Init(slotIdx);
                            SetItemPopUpPosition(itemPopUp);
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && itemPopUp != null && itemPopUp.gameObject.activeInHierarchy)
        {
            if (!IsPointerOverTransform(itemPopUp.transform))
            {
                CloseItemPopUp();
            }
        }
    }

    // 检查点击的格子id
    private int CheckClickSlotIdx()
    {
        int activePage = GetActivePageIndex();
        if (activePage < 0 || activePage >= pageSlots.Count)
            return -1;

        int pageBagSlotCount = GetPageBagSlotCount();
        if (pageBagSlotCount <= 0)
            return -1;

        int localIdx = CheckClickLocalSlotIdx(activePage, pageBagSlotCount);
        if (localIdx < 0)
            return -1;

        int globalIdx = activePage * pageBagSlotCount + localIdx;
        if (globalIdx < 0 || globalIdx >= BagManager.Instance.Items.Length)
            return -1;

        return globalIdx;
    }

    private int CheckClickLocalSlotIdx(int pageIndex, int pageBagSlotCount)
    {
        var pageSlotList = pageSlots[pageIndex];
        int maxSlots = Mathf.Min(pageSlotList.Count, pageBagSlotCount);
        for (int i = 0; i < maxSlots; i++)
        {
            if (IsPointerOverUI(pageSlotList[i].gameObject))
            {
                return i;
            }
        }
        return -1;
    }

    private int GetActivePageIndex()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null && pages[i].gameObject.activeInHierarchy)
                return i;
        }
        return 0;
    }

    private int GetPageBagSlotCount()
    {
        if (pages == null || pages.Length == 0)
            return 0;

        return Mathf.CeilToInt((float)BagManager.Instance.Items.Length / pages.Length);
    }

    private Image GetSlotByBagIndex(int bagIndex)
    {
        int pageBagSlotCount = GetPageBagSlotCount();
        if (pageBagSlotCount <= 0)
            return null;

        int pageIndex = bagIndex / pageBagSlotCount;
        int localIndex = bagIndex % pageBagSlotCount;
        if (pageIndex < 0 || pageIndex >= pageSlots.Count)
            return null;

        var pageSlotList = pageSlots[pageIndex];
        if (localIndex < 0 || localIndex >= pageSlotList.Count)
            return null;

        return pageSlotList[localIndex];
    }

    private void SetItemPopUpPosition(UIItemPopUp uiPopUp)
    {
        uiPopUp.SetScreenPosition(Input.mousePosition);
    }

    private bool IsPointerOverTransform(Transform target)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult r in results)
        {
            Transform t = r.gameObject.transform;
            if (t == target || t.IsChildOf(target))
                return true;
        }

        return false;
    }

    private void CloseItemPopUp()
    {
        if (itemPopUp != null && itemPopUp.gameObject.activeInHierarchy)
        {
            itemPopUp.Close();
        }
        itemPopUp = null;
    }

    /// <summary>
    /// 判断鼠标是否在Item上
    /// </summary>
    private bool IsPointerOverUI(GameObject uiElement)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult r in results)
        {
            Transform t = r.gameObject.transform;
            if (r.gameObject == uiElement || t.IsChildOf(uiElement.transform)) return true;
        }
        return false;
    }
}
