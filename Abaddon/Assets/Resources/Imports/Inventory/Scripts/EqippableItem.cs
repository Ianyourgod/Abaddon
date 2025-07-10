///----------------------------\\\
//  Ultimate Inventory Engine   \\
// Copyright (c) N-Studios. All Rights Reserved. \\
//      https://nikichatv.com/N-Studios.html	  \\
///-----------------------------------------------\\\
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqippableItem : MonoBehaviour
{
    [Header("Item's ID")]
    [Tooltip(
        "This is the ID of the item that will be equipped. Make sure it is the right one otherwise a wrong item may be equipped!"
    )]
    public int ItemID;
}
