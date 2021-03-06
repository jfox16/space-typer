using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShieldHealth : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;

    // Update is called once per frame
    void Update()
    {
        tmpText.text = TypingGameController.Instance.shieldHealth.ToString();
    }
}
