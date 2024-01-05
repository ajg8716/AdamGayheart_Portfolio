using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class InputController : MonoBehaviour
{

    /// <summary>
    /// finds if the w key is held down
    /// </summary>
    /// <returns></returns>
    public static bool isWDown()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// finds if the a key is being held down
    /// </summary>
    /// <returns></returns>
    public static bool isADown()
    {
        if (Keyboard.current.aKey.isPressed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// finds if the d key is being held down
    /// </summary>
    /// <returns></returns>
    public static bool isDDown()
    {
        if (Keyboard.current.dKey.isPressed)
        {
            return true;
        }
        return false;
    }

    //finds if the left mouse button was clicked
    public static bool isLeftMouseClicked()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// finds if enter key was clicked
    /// </summary>
    /// <returns></returns>
    public static bool isEnterClicked()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            return true;
        }
        return false;
    }

}
