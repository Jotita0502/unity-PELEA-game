using TMPro;
using UnityEngine;

public class PlayerSlotUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI skinText;
    public TextMeshProUGUI stateText;

    public void SetTitle(string t) => titleText.text = t;
    public void SetSkin(int index) => skinText.text = $"Skin {index}";
    public void SetReady(bool isReady) => stateText.text = isReady ? "Listo" : "No listo";
}
