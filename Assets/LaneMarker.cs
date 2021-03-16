using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaneMarker : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;

    public void SetText(string text)
    {
        tmpText.text = text;
    }
}
