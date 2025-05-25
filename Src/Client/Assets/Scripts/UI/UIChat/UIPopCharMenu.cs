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
    /// ��ʾʱ�����״̬Ϊѡ�У������ܴ���ȡ��ѡ���¼�
    /// </summary>
    public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3 (80, 0, 0);
    }

    /// <summary>
    /// ȡ��ѡ���¼�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        //  ֻ�е��Ե�ʱ����ܷ���eventData��PointerEventData�࣬BaseEventData���������¼���Ļ���
        var ed = eventData as PointerEventData;
        //  �������˰�ť�������򲻹رգ���֤��ť��������Ӧ
        //  ed.hovered�ǰ�����ǰ�������������������б�
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
            MessageBox.Show("��������Լ�Ϊ����Ŷ~");
            return;
        }
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
        this.Close(WindowResult.No);
    }
    public void InviteTeam()
    {
        MessageBox.Show(string.Format("ȷ��ѡ�����롾{0}�����������", targetName), "����������", MessageBoxType.Confirm, "����", "ȡ��").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteResquest(targetId, targetName);
        };
        this.Close(WindowResult.No);
    }

    

}
