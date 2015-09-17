﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[ RequireComponent(typeof(Button))]
public class RadialSlider : MonoBehaviour, IPointerDownHandler
{
    string m_SliderName = "Slider";
    Vector3 m_PressDownPos;
    Vector3 m_CurrentMousePos;

    bool m_Pressed = false;

    public Image m_SliderImage;
    Text m_Text;

    float m_PreNormVal;
    float m_NormalizedVal = 0;
    public Vector2 m_Range = new Vector2(0, 1);
    float m_ScaledVal;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    public FloatEvent OnDragUpdate;

    void Start()
    {
        m_Text = GetComponentInChildren<Text>();
        m_SliderImage.fillAmount = m_NormalizedVal;
    }

    void Update()
    {
        if( m_Pressed && Input.GetMouseButtonUp( 0 ) )
        {
            Unclick();
            m_Text.text = m_SliderName;
        }
        else if( m_Pressed )
        {
            m_CurrentMousePos = Input.mousePosition;
            float distance = m_CurrentMousePos.x - m_PressDownPos.x;
            distance /= Screen.width;

            m_NormalizedVal = m_PreNormVal + distance;
            m_NormalizedVal = Mathf.Clamp01(m_NormalizedVal);

            m_ScaledVal = m_Range.x + ( m_NormalizedVal * ( m_Range.y - m_Range.x ) );

            m_SliderImage.fillAmount = m_NormalizedVal;

            if (m_ScaledVal == 0)
                m_Text.text = "0";
            else
                m_Text.text = m_ScaledVal.ToString("##.##");

            print("Drag value: " + m_ScaledVal);
            OnDragUpdate.Invoke(m_ScaledVal);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was click drag started.");
        m_Pressed = true;
        m_PressDownPos = Input.mousePosition;
        m_PreNormVal = m_NormalizedVal;
    }
       

    void Unclick()
    {
        m_Pressed = false;
    }
}
