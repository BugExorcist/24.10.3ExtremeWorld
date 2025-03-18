using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using Models;

public class NPCController : MonoBehaviour
{
    public int npcID;

    SkinnedMeshRenderer renderer;
    Animator animator;
    Color orignColor;
    NpcDefine npc;

    private bool inInteractive = false;

    void Start()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        animator = this.gameObject.GetComponent<Animator>();
        orignColor = renderer.sharedMaterial.color;
        npc = NPCManager.Instance.GetNpcDefine(npcID);
        this.StartCoroutine(Actions());
    }

    IEnumerator Actions()
    {   
        while (true)
        {
            if (inInteractive)
                yield return new WaitForSeconds(2f);
            else
            {
                yield return new WaitForSeconds(Random.Range(8f, 10f));
                this.Relax();
            }
        }
    }

    private void Relax()
    {
        animator.SetTrigger("Relax");
    }

    private void Interactive()
    {
        if(!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();
        if (NPCManager.Instance.Interactive(npc))
        {
            animator.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;
    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }
    void OnMouseDown()
    {   //被鼠标点击时触发
        if (NearToNpc())
            Interactive();
    }

    private void OnMouseOver()
    {
        if (NearToNpc())
            Highlight(true);
    }

    private void OnMouseEnter()
    {
        if (NearToNpc())
            Highlight(true);
    }

    private void OnMouseExit()
    {
        if (NearToNpc())
            Highlight(false);
    }
    private void Highlight(bool highlight)
    {
        if(highlight)
        {
            if (renderer.sharedMaterial.color != Color.white)
                renderer.sharedMaterial.color = Color.white;
        }
        else
        {
            if (renderer.sharedMaterial.color != orignColor)
                renderer.sharedMaterial.color = orignColor;
        }
    }

    private bool NearToNpc()
    {
        // 使用 Vector3.SqrMagnitude 比较距离的平方，避免开平方运算
        return Vector3.SqrMagnitude(User.Instance.CurrentCharacterObject.transform.position - this.transform.position) < 6 * 6;
    }
}