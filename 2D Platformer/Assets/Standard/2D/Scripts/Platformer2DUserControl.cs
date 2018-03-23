using System;
using UnityEngine;


namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
        private bool m_Roll;
        private bool m_Crouch;
        private float h;
        private ParticleSystem cannon = null;

        private void Awake()
        {
            cannon = GetComponentInChildren<ParticleSystem>();
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            // Read the inputs.
            h = Input.GetAxis("Horizontal");
            m_Crouch = Input.GetKey(KeyCode.LeftControl);
            m_Roll = Input.GetKeyDown(KeyCode.LeftShift);
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = Input.GetButtonDown("Jump");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                cannon.Emit(1);
            }
        }


        private void FixedUpdate()
        {
            // Pass all parameters to the character control script.
            m_Character.Move(h, m_Crouch, m_Jump, m_Roll);
            m_Jump = false;
        }
    }
}
