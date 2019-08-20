using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionNumber : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text text = GetComponent<Text>();
        text.text = "v." + Application.version;
    }
}
