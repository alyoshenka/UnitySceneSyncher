using UnityEngine;

struct GameDev
{
    // user configurable variables
    string name;

    // extra variables
    Color displayColor;
    Sprite displayIcon;

    // constant variables
    Vector3 position;
    Quaternion rotation;
    GameObject currentlySelectedObject;
}

// ability to "lock" item in hierarchy
    // or change its color to indicate it shouldn't be moved by anyone else
