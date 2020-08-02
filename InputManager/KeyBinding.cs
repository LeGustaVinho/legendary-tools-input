using System;
using UnityEngine;

namespace LegendaryTools.Input
{
    public enum MouseButton
    {
        None = -1,
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public enum MouseEvent
    {
        Press,
        Release,
        Click,
        Hold
    }

    [Serializable]
    public class KeyBinding
    {
        public KeyCode AltNegativeKey = KeyCode.None;

        public KeyCode AltPositiveKey = KeyCode.None;

        [NonSerialized] public bool FrameKeyDown;

        [NonSerialized] public bool FrameKeyUp;

        public float Gravity = 1;
        public bool Invert;

        private float m_Value;
        public string Name;
        public KeyCode NegativeKey = KeyCode.None;

        public KeyCode PositiveKey = KeyCode.None;
        public float Sensivity = 1;

        public bool Snap;

        public KeyBinding()
        {
        }

        public KeyBinding(string name, KeyCode negativeKey, KeyCode positiveKey, float sensivity, float gravity,
            bool invert = false)
        {
            Name = name;
            NegativeKey = negativeKey;
            PositiveKey = positiveKey;
            Sensivity = sensivity;
            Gravity = gravity;
            Invert = invert;
        }

        public KeyBinding(string name, KeyCode negativeKey, KeyCode positiveKey, KeyCode altNegativeKey,
            KeyCode altPositiveKey, float sensivity, float gravity, bool invert = false)
        {
            Name = name;
            NegativeKey = negativeKey;
            PositiveKey = positiveKey;
            AltNegativeKey = altNegativeKey;
            AltPositiveKey = altPositiveKey;
            Sensivity = sensivity;
            Gravity = gravity;
            Invert = invert;
        }

        public float Value
        {
            get
            {
                if (Invert)
                {
                    return m_Value * -1;
                }

                return m_Value;
            }
        }

        public void RaiseValue()
        {
            if (m_Value < 0 && Snap)
            {
                m_Value = 0;
            }

            if (m_Value == 0)
            {
                FrameKeyDown = true;
            }

            m_Value = Mathf.Clamp(m_Value + Sensivity * Time.deltaTime, -1, 1);
        }

        public void LowerValue()
        {
            if (m_Value > 0 && Snap)
            {
                m_Value = 0;
            }

            m_Value = Mathf.Clamp(m_Value - Gravity * Time.deltaTime, -1, 1);
        }

        public void MoveToNeutral()
        {
            if (Snap)
            {
                if (m_Value != 0)
                {
                    FrameKeyUp = true;
                }

                m_Value = 0;
                return;
            }

            if (Value > 0)
            {
                if (Value - Sensivity < 0)
                {
                    m_Value = 0;
                    FrameKeyUp = true;
                }
                else
                {
                    LowerValue();
                }
            }
            else if (Value < 0)
            {
                if (Value + Sensivity > 0)
                {
                    m_Value = 0;
                    FrameKeyUp = true;
                }
                else
                {
                    RaiseValue();
                }
            }
        }

        public static MouseButton ToMouseButton(KeyCode key)
        {
            return (MouseButton) Mathf.Clamp((int) key - (int) KeyCode.Mouse0, (int) MouseButton.None,
                (int) MouseButton.Middle);
        }

        public static KeyCode ToKeyCode(MouseButton mouseButton)
        {
            return (KeyCode) Mathf.Clamp((int) mouseButton + (int) KeyCode.Mouse0, (int) KeyCode.Mouse0,
                (int) KeyCode.Mouse6);
        }

        public MouseButton PositiveKeyToMouseButton()
        {
            return ToMouseButton(PositiveKey);
        }

        public MouseButton NegativeKeyToMouseButton()
        {
            return ToMouseButton(NegativeKey);
        }

        public MouseButton AltPositiveKeyToMouseButton()
        {
            return ToMouseButton(AltPositiveKey);
        }

        public MouseButton AltNegativeKeyToMouseButton()
        {
            return ToMouseButton(AltNegativeKey);
        }

        public void ResetFrameKeys()
        {
            FrameKeyUp = false;
            FrameKeyDown = false;
        }
    }
}