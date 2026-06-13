using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapCardUI : MonoBehaviour
{
    public Image bg;
    public Image artwork;
    public TMP_Text titleText;
    public Button button; // opcional (para clic con mouse)

    public void Set(Sprite sprite, string title)
    {
        if (artwork) artwork.sprite = sprite;
        if (titleText) titleText.text = title;
    }

    public void SetSelected(bool selected)
    {
        if (bg)
        {
            var c = bg.color;
            c.a = selected ? 1f : 0.55f;
            bg.color = c;
        }
        transform.localScale = selected ? Vector3.one * 1.03f : Vector3.one;
    }
}
