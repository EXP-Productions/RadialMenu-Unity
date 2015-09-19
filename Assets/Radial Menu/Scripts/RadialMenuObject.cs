using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class RadialMenuObject : MonoBehaviour, IPointerDownHandler
{
    public string       m_ObjectName;
    public GameObject   m_ObjectToCall;
    public string       m_FunctionToCall;

    float m_Fade;

    public Image[] m_FGImages;
    public Image[] m_BGImages;
    public Image[] m_HLImages;
    public Text    m_Text;

    public Color m_FGCol;
    public Color m_BGCol;
    public Color m_HLCol;
    public Color m_TextCol;

    protected RadialMenu m_RadMenu;


    public virtual void Init(  RadialMenu radMenu, string name, GameObject objectToCall, string functionToCall )
    {
        m_RadMenu = radMenu;
        m_ObjectName = name;
        m_ObjectToCall = objectToCall;
        m_FunctionToCall = functionToCall;
    }

    public void SetPallette( Color fgCol, Color bgCol, Color hlCol, Color textCol )
    {
        m_FGCol = fgCol;
        m_BGCol = bgCol;
        m_HLCol = hlCol;
        m_TextCol = textCol;

        UpdateCols();
    }

    public void Fade( float fade )
    {
        m_Fade = fade;
        print(name + "Fade set too: " + fade);

        m_FGCol.a = fade;
        m_BGCol.a = fade;
        m_HLCol.a = fade;
        m_TextCol.a = fade;

        UpdateCols();     
    }

    void UpdateCols()
    {
        foreach( Image i in m_FGImages ) i.color = m_FGCol;
        foreach( Image i in m_BGImages ) i.color = m_BGCol;
        foreach( Image i in m_HLImages ) i.color = m_HLCol;

        m_Text.color = m_TextCol;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }

    protected void Disengage()
    {
        m_RadMenu.DisengageSelection();
    }

    
	
    public virtual void CallFunction()
    {
        // do what ever the object does here
    }
}
