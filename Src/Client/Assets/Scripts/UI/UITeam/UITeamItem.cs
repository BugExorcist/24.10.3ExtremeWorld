using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SkillBridge.Message;
using Entities;
using Managers;

public class UITeamItem : ListView.ListViewItem
{
    public TMP_Text info;
    public Image classIcon;
    public Image leaderIcon;
    public Image background;
    private Sprite normalBackGround;
    public Sprite selectedBackGround;
    public Slider HPBar;


    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBackGround : normalBackGround;
    }

    public int idx;
    public NCharacterInfo chaInfo;
    private Creature character;

    private void Start()
    {
        this.normalBackGround = background.GetComponent<Sprite>();
    }

    public void SetMemnerInfo(int idx, NCharacterInfo info, bool isLeader)
    {
        this.idx = idx;
        this.chaInfo = info;
        this.leaderIcon.gameObject.SetActive(isLeader);
        if (this.name != null) this.info.text = string.Format("Lv.{0} {1}",this.chaInfo.Level, this.chaInfo.Name);
        if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.chaInfo.Class];
        this.character = EntityManager.Instance.GetEntity(info.EntityId) as Creature;
    }

    private void Update()
    {
        if (this.HPBar != null)
        {
            this.HPBar.maxValue = character.Attributes.MaxHP;
            this.HPBar.value = character.Attributes.HP;
        }
    }
}
