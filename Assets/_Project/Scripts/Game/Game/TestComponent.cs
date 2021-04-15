using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class TestComponent : MonoBehaviour
{
    [RangeSlider(-9, 15)]
    public Range range;

    public Pair<int, float> pair;

    private void Start()
    {
        this.ExecuteAfter(1, () => { Debug.Log("Delayed Execution"); });
    }
}
