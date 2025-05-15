using Services;
using TMPro;

public class UIGuildPopCreate : UIWindow
{
    public TMP_InputField inputName;
    public TMP_InputField inputNotice;

    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult += OnGuildCreated;
    }

    private void OnDistory()
    {
        GuildService.Instance.OnGuildCreateResult -= OnGuildCreated;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入工会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (inputName.text.Length < 2 || inputName.text.Length > 10)
        {
            MessageBox.Show("工会名称为2-10个字符", "错误", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入工会宗旨", "错误", MessageBoxType.Error);
            return;
        }
        if (inputNotice.text.Length < 3 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("工会宗旨为3-50个字符", "错误", MessageBoxType.Error);
            return;
        }
        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);
    }

    void OnGuildCreated(bool result)
    {
        if (result)
            this.Close(WindowResult.Yes);
    }
}
