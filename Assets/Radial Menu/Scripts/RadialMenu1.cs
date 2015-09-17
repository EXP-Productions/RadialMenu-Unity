/*
Radial Menu by XY01 (Brad Hammond) - http://www.XY01.net
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Radial Menu
///  - Pass in array of strings to populate the menu
///  - Fires event with index of selected element
///  
/// TODO:
///  - Clean up messy code
///  - Take pictures instead of strings
///  - Comment code
///  - Make so you can populate in editor with buttons that can have individual events
///  
///  RadialMenuObjects 
///      
/// 
/// CreateMenu ( 
/// sliderX( objectToCall, funtionsToCall, min, max, startval ),
/// toggle( objectToCall, funtionsToCall, initialBool ),
/// button( objectToCall, funtionsToCall )
/// }
/// </summary>
/// 
[RequireComponent(typeof(Button))]
public class RadialMenu1 : MonoBehaviour, IPointerDownHandler
{
    enum State
    {
        Deactivated,
        Activating,
        Active,
        Deactivating,
    }

    // State of the menu
    State m_State = State.Deactivated;

    // Radial layout - positions the elements
    RadialLayout m_RadLayout;

    // Prefab buttons that will populate the menu
    public RectTransform m_DefaultMenuObject;

    public RadialSlider1    m_SliderPrefab;
    public RadialMenuObject m_ButtonPrefab;
    public RadialMenuObject m_TogglePrefab;
    public RadialList       m_ListPrefab;


    // Name displayed on the menu
    public string m_MenuName = "Menu";
    Text m_MenuText;

    // Main menu button
    Button MainButton;

    // List of buttons
    public List<RadialMenuObject> m_MenuObjects = new List<RadialMenuObject>();


    // Size in pixels of buttons
    public float m_ButtonSizeMain = 60;
    public float m_ButtonSizeChild = 60;

    // Radius in pixels that the buttons will move out too
    public float m_Radius = 100;   

    // Area within which no selection will register, giving you a way to cancel the action
    public float m_DeadZone = 30;

    // Display the name of the last selected element or menu name after selection
    public bool m_DisplaySelectedName = false;

    // Target radius for the buttons
    float m_TargetRadius = 0;

    // Smoothing on the radius lerp
    public float m_Smoothing = 8;

    // Current index of the selected element
    int m_SelectedIndex = 0;

    // flag for element being selected or not
    bool m_OptionSelected = false;

    // Anlge and range for the layout
    public float m_StartAngle = 0;
    public float m_AngleRange = 360;


    // Unity event which fires off the selected index
    [System.Serializable]
    public class SelectionEvent : UnityEvent<int> { }
    public SelectionEvent OnSelected;

    // Test array of events
    public SelectionEvent[] OnSelectedEvents;

    public Color m_HighlightCol;
    public Color m_BaseCol;

    public Color m_TextColBase;
    public Color m_TextColHighlight;

    GameObject m_ObjectToMessage;
    string m_FunctionCall;


    public bool m_HideInactive = false;

	void Start () 
    {
        MainButton = GetComponent<Button>();
        m_MenuText = GetComponentInChildren<Text>();
        m_MenuText.text = m_MenuName;

        m_RadLayout = new GameObject("Radial Layout").AddComponent<RadialLayout>();
        m_RadLayout.transform.SetParent(transform);
        m_RadLayout.transform.localPosition = Vector3.zero;
        m_RadLayout.MaxAngle = m_AngleRange;
        m_RadLayout.StartAngle = m_StartAngle;

        int index = m_RadLayout.transform.GetSiblingIndex();
        m_RadLayout.transform.SetSiblingIndex(index - 1);

        SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
	}


    // Transition to pooling later
    public void Reset()
    {
        foreach (RadialMenuObject obj in m_MenuObjects)
            Destroy(obj.gameObject);

        m_MenuObjects.Clear();
    }

    void Update()
    {
        AdjustAngleBasedOnScreenPos();

        // Update selection colours
       
        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            if (!m_MenuObjects[i].gameObject.activeSelf)
                continue;

            if( !m_OptionSelected )
            {
               // m_MenuObjects[i].image.color = Color.Lerp(m_MenuObjects[i].image.color, m_BaseCol, Time.deltaTime * m_Smoothing);
                m_MenuObjects[i].GetComponentInChildren<Text>().color = m_TextColBase;
            }
            else if( i != m_SelectedIndex )
            {
               // m_MenuObjects[i].image.color = Color.Lerp(m_MenuObjects[i].image.color, m_BaseCol, Time.deltaTime * m_Smoothing);
                m_MenuObjects[i].GetComponentInChildren<Text>().color = m_TextColBase;
            }
            else
            {              
              //  m_MenuObjects[m_SelectedIndex].image.color = Color.Lerp(m_MenuObjects[m_SelectedIndex].image.color, m_HighlightCol, Time.deltaTime * m_Smoothing);
                m_MenuObjects[i].GetComponentInChildren<Text>().color = m_TextColHighlight;
            }
        }

        if (m_State == State.Activating || m_State == State.Active)
        {
            if (m_OptionSelected) MainButton.image.color = Color.Lerp(MainButton.image.color, m_HighlightCol, Time.deltaTime * m_Smoothing);
        }
        else MainButton.image.color = Color.Lerp(MainButton.image.color, m_BaseCol, Time.deltaTime * m_Smoothing);

        if( m_State == State.Activating )
        {
            m_RadLayout.UpdateFDistance(Mathf.Lerp(m_RadLayout.fDistance, m_TargetRadius, Time.deltaTime * m_Smoothing));

             float norm = m_RadLayout.fDistance / m_Radius;
             SetButtonFade( Mathf.Pow( norm, 5) );

             if (Mathf.Abs(m_RadLayout.fDistance - m_TargetRadius) < 2)
             {
                 m_State = State.Active;
             }
        }
        else if (m_State == State.Active )
        {
            if (Vector3.Distance(Input.mousePosition, transform.position) < m_DeadZone)
            {
                m_MenuText.text = m_MenuName;
                m_OptionSelected = false;
            }
            else
            {
                m_SelectedIndex = FindClosestButtonIndex();
                m_OptionSelected = true;
            }

           // if (Input.GetMouseButtonUp(1))
           //     DeactivateMenu();
           
        }
        else if (m_State == State.Deactivating )
        {
            float norm = ( m_Radius - m_RadLayout.fDistance ) / m_Radius;
            norm = 1 - norm;
            SetButtonFade(Mathf.Pow(norm, 5));

            m_RadLayout.UpdateFDistance(Mathf.Lerp(m_RadLayout.fDistance, m_TargetRadius, Time.deltaTime * m_Smoothing));

            if (Mathf.Abs(m_RadLayout.fDistance - m_TargetRadius) < 2)
            {                
                m_State = State.Deactivated;
                for (int i = 0; i < m_MenuObjects.Count; i++)
                {
                    m_MenuObjects[i].gameObject.SetActive(false);
                }

                if (m_HideInactive)
                    transform.position = Vector3.right * float.MaxValue;
            }
        }
    }

    public void AddSlider( string name, GameObject objectToCall, string functionToCall, float rangeMin, float rangeMax, float initialValue )
    {
        RadialSlider1 slider = Instantiate( m_SliderPrefab );
        slider.Init(name, objectToCall, functionToCall);
        slider.m_Range = new Vector2(rangeMin, rangeMax);
        slider.ScaledVal = initialValue;
        slider.m_Text.text = name;

        slider.transform.SetParent(m_RadLayout.transform);

        m_MenuObjects.Add(slider);
    }

    public void AddList(string name, GameObject objectToCall, string functionToCall, string[] names )
    {
        RadialList list = Instantiate(m_ListPrefab);
        list.Init(name, objectToCall, functionToCall);
        list.GenerateMenu(name, names);
        list.transform.SetParent(m_RadLayout.transform);

        m_MenuObjects.Add(list);
    }

    public void SetPosition( Vector3 pos )
    {
      //  MainButton.siz
        pos.x = Mathf.Clamp(pos.x, m_ButtonSizeMain / 2, Screen.width - m_ButtonSizeMain / 2 );
        pos.y = Mathf.Clamp(pos.y, m_ButtonSizeMain / 2, Screen.height - m_ButtonSizeMain / 2);

        transform.position = pos;
    }

    void AdjustAngleBasedOnScreenPos()
    {
        // Check if radius is outside of screen
        bool leftClip = false;
        bool rightClip = false;
        bool topClip = false;
        bool bottomClip = false;

        if ((transform.position - (Vector3.right * m_Radius)).x - (m_ButtonSizeChild/2f) < 0 ) leftClip = true;
        if ((transform.position + (Vector3.right * m_Radius)).x + (m_ButtonSizeChild / 2f) > Screen.width) rightClip = true;
        if ((transform.position + (Vector3.up * m_Radius)).y + (m_ButtonSizeChild / 2f) > Screen.height) topClip = true;
        if ((transform.position - (Vector3.up * m_Radius)).y - (m_ButtonSizeChild / 2f) < 0) bottomClip = true;


        if (leftClip && !topClip && !bottomClip )
        {
            m_RadLayout.StartAngle = 90;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if (rightClip && !topClip && !bottomClip)
        {
            m_RadLayout.StartAngle = 270;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if (topClip && !rightClip && !leftClip)
        {
            m_RadLayout.StartAngle = 0;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if (bottomClip && !rightClip && !leftClip)
        {
            m_RadLayout.StartAngle = 180;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if ( bottomClip && rightClip )
        {
            m_RadLayout.StartAngle = 180;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else if (bottomClip && leftClip)
        {
            m_RadLayout.StartAngle = 90;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else if (topClip && rightClip)
        {
            m_RadLayout.StartAngle = 270;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else if (topClip && leftClip)
        {
            m_RadLayout.StartAngle = 0;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else
        {
            m_RadLayout.StartAngle = 0;
            m_RadLayout.MaxAngle = 360;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
    }

    int FindClosestButtonIndex()
    {
        int closestButtonIndex = 0;
        float closestDist = float.MaxValue;
        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            float dist = Vector3.Distance(Input.mousePosition, m_MenuObjects[i].transform.position);
            if( dist < closestDist )
            {
                closestDist = dist;
                closestButtonIndex = i;
            }
        }

        return closestButtonIndex;
    }

    void SetButtonSize( float mainSize, float childSize )
    {
        MainButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,   mainSize);
        MainButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,     mainSize);

        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            m_MenuObjects[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childSize);
            m_MenuObjects[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childSize);
        }
    }

    void SetButtonFade( float fade )
    {
        /*
        for (int i = 0; i < m_ButtonsNames.Length; i++)
        {
            ColorBlock colBlock = m_MenuObjects[i].colors;

            Color currentCol = colBlock.normalColor;
            currentCol.a = fade;
            colBlock.normalColor = currentCol;

            m_MenuObjects[i].colors = colBlock;

            Color textCol = m_MenuObjects[i].GetComponentInChildren<Text>().color;
            textCol.a = fade;
            m_MenuObjects[i].GetComponentInChildren<Text>().color = textCol;
        }
         * */
    }
	
    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_State == State.Deactivated )
        {
            ActivateMenu();
            Debug.Log(this.gameObject.name + " Was Clicked.");
        }
        else if( m_State == State.Active )
        {
            DeactivateMenu();
            print("here");
        }
    }
    
    public void ActivateMenu()
    {
        print("Menu activated");

        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            m_MenuObjects[i].gameObject.SetActive(true);
        }

        m_State = State.Activating;

        m_TargetRadius = m_Radius;
    }

    void DeactivateMenu()
    {
        if (m_OptionSelected)
        {
            OnSelected.Invoke(m_SelectedIndex);
            if( m_ObjectToMessage != null )
                m_ObjectToMessage.SendMessage(m_FunctionCall, m_SelectedIndex);
        }

        m_State = State.Deactivating;

        if ( !m_DisplaySelectedName )
            m_MenuText.text = m_MenuName;

        print("Deactivating,    Name set to : " + m_MenuText.text);

        m_TargetRadius = 0;
    }

    RectTransform CreateNewButton()
    {
        RectTransform newBtn = Instantiate(m_DefaultMenuObject) as RectTransform;
        newBtn.transform.SetParent(m_RadLayout.transform);
        newBtn.gameObject.SetActive(false);

        newBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_ButtonSizeChild);
        newBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ButtonSizeChild);

        newBtn.name = "Rad Btn " + m_MenuObjects.Count;

        return newBtn;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_Radius);
    }
}
