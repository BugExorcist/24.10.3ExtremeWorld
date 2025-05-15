using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem> { }

public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> onItemSelected;
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }

        /// <summary>
        /// 继承自ListViewItem需要重写此方法实现选中的逻辑
        /// </summary>
        /// <param name="selected"></param>
        public virtual void onSelected(bool selected)
        {
        }

        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)
            {
                this.Selected = true;
            }
            if (owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }
    }

    List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedIten = null;
    public ListViewItem SelectedItem
    {
        get { return selectedIten; }
        set
        {
            if (selectedIten != null && selectedIten != value)
            {
                selectedIten.Selected = false;
            }
            selectedIten = value;
            onItemSelected?.Invoke(value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    public void RemoveAll()
    {
        foreach (var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }

    public void InitItemStatus()
    {
        if (this.SelectedItem != null)
        {
            this.SelectedItem.Selected = false;
            this.SelectedItem = null;
        }
    }
}