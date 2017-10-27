using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonPattern;

public class CInputManager : CSingletonPattern<CInputManager>
{
    public const float SLIDE_RATIO = 0.05f;
    public const float LONG_PRESSED_TIME = 0.3f;

    Vector2 slideStartPos;
    Vector2 prevPos;

    Vector2 pressedPos;
    public Vector2 PressedPos
    {
        get
        {
            return pressedPos;
        }
        private set
        {
            pressedPos = value;
        }
    }

    bool moved = false;
    bool pressed = false;
    bool longPressed = false;

    float pressedTime = 0.0f;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            slideStartPos = GetPressedPosition();

            pressedTime = 0.0f;
        }
#elif UNITY_ANDROID
        if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            slideStartPos = GetPressedPosition();

            pressedTime = 0.0f;
        }
#endif
    }


    public bool Pressed()
    {
#if UNITY_EDITOR
        if (!moved && !longPressed && Input.GetMouseButtonDown(0))
        {
            pressed = true;
            pressedPos = GetPressedPosition();
        }

#elif UNITY_ANDROID
        if (!moved && !longPressed && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            pressed = true;
            pressedPos = GetPressedPosition();
        }
#endif
        else
            pressed = false;

        return pressed;
    }

    public Vector2 GetPressedPosition()
    {
        Vector2 pos;

#if UNITY_EDITOR
        pos = Input.mousePosition;
#elif UNITY_ANDROID
        pos = Input.GetTouch(0).position;
#endif

        return pos;
    }

    public bool LongPressed()
    {
        if (!moved && pressedTime > LONG_PRESSED_TIME)
            longPressed = true;
        else
            longPressed = false;

        return longPressed;
    }

}
