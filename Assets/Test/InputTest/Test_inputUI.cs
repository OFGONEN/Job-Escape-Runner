using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;

public class Test_inputUI : MonoBehaviour
{
    public SharedVector3 inputDirection;
    public TextMeshProUGUI textRenderer;

    // Update is called once per frame
    void Update()
    {
        textRenderer.text = inputDirection.sharedValue.ToString();
    }
}
