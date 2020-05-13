using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartGame.KartSystems
{
    public class MobileInput : MonoBehaviour, IInput, Server_CSharp.InputMovileInterface
    {
        private int widthScreen;
        private int heightScreen;

        public float Acceleration
        {
            get { return m_Acceleration; }
        }
        public float Steering
        {
            get { return m_Steering; }
        }
        public bool BoostPressed
        {
            get { return m_BoostPressed; }
        }
        public bool FirePressed
        {
            get { return m_FirePressed; }
        }
        public bool HopPressed
        {
            get { return m_HopPressed; }
        }
        public bool HopHeld
        {
            get { return m_HopHeld; }
        }

        float m_Acceleration;
        float m_Steering;
        bool m_HopPressed;
        bool m_HopHeld;
        bool m_BoostPressed;
        bool m_FirePressed;

        bool m_FixedUpdateHappened;

        // Start is called before the first frame update
        void Start()
        {

        }

        void FixedUpdate()
        {
            m_FixedUpdateHappened = true;
        }

        public bool RecieveTouch(int x, int y, int typeOfPress, ref bool vibrate)
        {
            
            if (Input.GetKey(KeyCode.UpArrow))
                m_Acceleration = 1f;
            else if (Input.GetKey(KeyCode.DownArrow))
                m_Acceleration = -1f;
            else
                m_Acceleration = 0f;

            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                m_Steering = -1f;
            else if (!Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
                m_Steering = 1f;
            else
                m_Steering = 0f;

            m_HopHeld = Input.GetKey(KeyCode.Space);

            if (m_FixedUpdateHappened)
            {
                m_FixedUpdateHappened = false;

                m_HopPressed = false;
                m_BoostPressed = false;
                m_FirePressed = false;
            }

            m_HopPressed |= Input.GetKeyDown(KeyCode.Space);
            m_BoostPressed |= Input.GetKeyDown(KeyCode.RightShift);
            m_FirePressed |= Input.GetKeyDown(KeyCode.RightControl);


            vibrate = false;
            return true;
        }

        public bool EndOfConection()
        {
            throw new System.NotImplementedException();
        }

        public bool ScreenSize(int width, int height)
        {
            widthScreen = width;
            heightScreen = height;
            return true;
        }
    }
}