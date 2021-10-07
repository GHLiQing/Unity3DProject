using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AlphaHitTestImage : Image
{
    [SerializeField]
    public float alphaHitTest = 0.1f;

    private float currentAlphaHitTest;
    protected override void Awake()
    {
        base.Awake();
        currentAlphaHitTest = alphaHitTest;
        this.alphaHitTestMinimumThreshold = currentAlphaHitTest;
    }
    private void Update()
    {
        if (alphaHitTest < 0)
            alphaHitTest = 0;
        else if (alphaHitTest > 1)
            alphaHitTest = 1;

        if (currentAlphaHitTest != alphaHitTest)
        {
            currentAlphaHitTest = alphaHitTest;
            this.alphaHitTestMinimumThreshold = currentAlphaHitTest;
        }
    }
}
