using TMPro;
using UnityEngine;


internal class UIPopupText : MonoBehaviour
{
    public TMP_Text normalDmgText;
    public TMP_Text critDmgText;
    public TMP_Text healText;
    public float floatTime = 0.5f;

    public void InitPopup(PopupType type, float number, bool isCrit)
    {
        string text = number.ToString("0");
        normalDmgText.text = text;
        critDmgText.text = text;
        healText.text = text;

        normalDmgText.enabled = !isCrit && number < 0;
        critDmgText.enabled = isCrit && number < 0;
        healText.enabled = number > 0;

        float time = Random.Range(0f, 0.5f) + floatTime;
        float height = Random.Range(0.5f, 1f);
        float disperse = Random.Range(-0.5f, 0.5f);
        disperse += Mathf.Sign(disperse) * 0.3f;

        LeanTween.moveX(this.gameObject, this.transform.position.x + disperse, time);
        LeanTween.moveZ(this.gameObject, this.transform.position.z + disperse, time);
        LeanTween.moveY(this.gameObject, this.transform.position.y + height, time).setEaseOutBack().setDestroyOnComplete(true);
    }
}
