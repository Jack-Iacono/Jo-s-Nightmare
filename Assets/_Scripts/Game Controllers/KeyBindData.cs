using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindData
{
    // These will be used for setting keys if we ever add keybind setting
    public enum KeyAction
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        JUMP,
        HIT,
        SHOOT,
        INTERACT,
        PAUSE,
        DANCE
    };

    public KeyCode keyForward = KeyCode.W;
    public KeyCode keyBackward = KeyCode.S;
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;
    public KeyCode keyJump = KeyCode.Space;

    public KeyCode keyHit = KeyCode.Mouse0;
    public KeyCode keyShoot = KeyCode.Mouse1;
    public KeyCode keyInteract = KeyCode.E;

    public KeyCode keyPause = KeyCode.Escape;

    public KeyCode keyDance = KeyCode.P;

    public KeyBindData()
    {
        keyForward = KeyCode.W;
        keyBackward = KeyCode.S;
        keyLeft = KeyCode.A;
        keyRight = KeyCode.D;
        keyJump = KeyCode.Space;

        keyHit = KeyCode.Mouse0;
        keyShoot = KeyCode.Mouse1;
        keyInteract = KeyCode.E;

        keyPause = KeyCode.Escape;

        keyDance = KeyCode.P;
    }
    
    public void SetKey(KeyAction name, KeyCode key)
    {
        switch (name)
        {
            case KeyAction.FORWARD:
                keyForward = key;
                break;
            case KeyAction.BACKWARD:
                keyBackward = key;
                break;
            case KeyAction.LEFT:
                keyLeft = key;
                break;
            case KeyAction.RIGHT:
                keyRight = key;
                break;
            case KeyAction.JUMP:
                keyJump = key;
                break;
            case KeyAction.HIT:
                keyHit = key;
                break;
            case KeyAction.SHOOT:
                keyShoot = key;
                break;
            case KeyAction.INTERACT:
                keyInteract = key;
                break;
            case KeyAction.PAUSE:
                keyPause = key;
                break;
            case KeyAction.DANCE:
                keyDance = key;
                break;
            default:
                Debug.Log("Unrecognize Key");
                break;
        }
    }
}
