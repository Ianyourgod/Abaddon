///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using UnityEngine;

public class HotbarSlot : MonoBehaviour
{
    private void Start()
    {
        if (!transform.parent.GetComponent<HotbarParent>())
            transform.parent.gameObject.AddComponent<HotbarParent>();
    }
}
