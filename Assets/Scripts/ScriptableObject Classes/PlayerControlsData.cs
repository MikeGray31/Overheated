using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerControlData", menuName = "Player/ControlData")]
public class PlayerControlsData : ScriptableObject
{
    public KeyCode StressModeKey = KeyCode.Mouse0;
    public KeyCode StressModeKeyAlt = KeyCode.LeftShift;
    public KeyCode JumpKey = KeyCode.Space;
}
