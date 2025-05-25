using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow ,IDeselectHandler
{
    public int targetId;
    public string targetName;
    /// <summary>
    /// 显示时，组件状态为选中，否则不能触发取消选中事件
    /// </summary>
    public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3 (80, 0, 0);
    }

    /// <summary>
    /// 取消选中事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        //  只有调试的时候才能发现eventData是PointerEventData类，BaseEventData类是所有事件类的基类
        var ed = eventData as PointerEventData;
        //  如果点击了按钮的区域，则不关闭，保证按钮正常的响应
        //  ed.hovered是包含当前组件及其所有子组件的列表
        if (ed.hovered.Contains(this.gameObject))
            return;
        this.Close(WindowResult.None);
    }


    public void OnChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void AddFriend()
    {
        if (targetId == User.Instance.CurrentCharacterInfo.Id)
        {
            MessageBox.Show("不能添加自己为好友哦~");
            return;
        }
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
        this.Close(WindowResult.No);
    }
    public void InviteTeam()
    {
        MessageBox.Show(string.Format("确定选择邀请【{0}】加入队伍吗？", targetName), "邀请好友组队", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteResquest(targetId, targetName);
        };
        this.Close(WindowResult.No);
    }

    

}
