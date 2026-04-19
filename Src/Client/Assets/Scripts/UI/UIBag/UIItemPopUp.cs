using Common.Data;
using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemPopUp : UIWindow
{
    private int slotIdx;
    public Button useButton;
    public Button spliteButton;
    public Button dropButotn;
    private RectTransform panelRect;

    private void EnsurePanelRect()
    {
        if (panelRect == null)
        {
            var panel = transform.Find("Root/bg");
            if (panel != null)
            {
                panelRect = panel.GetComponent<RectTransform>();
            }
        }
    }

    public void SetScreenPosition(Vector2 screenPos)
    {
        EnsurePanelRect();
        if (panelRect == null)
            return;

        RectTransform rootRect = panelRect.parent as RectTransform;
        Canvas canvas = GetComponent<Canvas>();
        Camera uiCamera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        Vector2 localPos;
        if (rootRect != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPos, uiCamera, out localPos))
        {
            panelRect.anchoredPosition = localPos + new Vector2(94, -75);
        }
        else
        {
            panelRect.position = screenPos;
        }
    }

    public void Init(int slotIndex)
    {
        this.slotIdx = slotIndex;
    }

    public void OnUse()
    {
        if (!TryGetBagItem(out var bagItem))
            return;

        ItemService.Instance.SendUseItem(this.slotIdx, 1);
        this.Close();
    }

    public void OnSplite()
    {
        if (!TryGetBagItem(out var bagItem))
            return;

        if (bagItem.Count < 2)
        {
            MessageBox.Show("数量不足，无法拆分", "提示");
            return;
        }

        int toSlot = BagManager.Instance.GetFirstEmptySlot();
        if (toSlot < 0)
        {
            MessageBox.Show("没有空背包格子，无法拆分", "提示");
            return;
        }

        int splitCount = bagItem.Count / 2;
        if (splitCount <= 0)
            splitCount = 1;

        ItemService.Instance.SendSplitItem(this.slotIdx, toSlot, splitCount);
        this.Close();
    }

    public void OnDrop()
    {
        if (!TryGetBagItem(out var bagItem))
            return;

        ItemService.Instance.SendDropItem(this.slotIdx, 1);
        this.Close();
    }

    private bool TryGetBagItem(out BagItem bagItem)
    {
        bagItem = default(BagItem);
        if (BagManager.Instance.Items == null)
            return false;
        if (this.slotIdx < 0 || this.slotIdx >= BagManager.Instance.Items.Length)
            return false;

        bagItem = BagManager.Instance.Items[this.slotIdx];
        return bagItem.ItemId > 0 && bagItem.Count > 0;
    }
}
