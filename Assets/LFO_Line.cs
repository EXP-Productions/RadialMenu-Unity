using UnityEngine;
using System.Collections;

public class LFO_Line : MonoBehaviour
{
    LineRenderer m_Line;
    int m_NumberOfPoints = 200;

    public LFO m_LFO;
    public float m_Length;
    public float m_DisplayFreq = 1;

    public float m_DisplayAmp = 1;

    public RadialMenu m_RadialMenu;

	// Use this for initialization
	void Start () 
    {
        m_Line = GetComponent<LineRenderer>();
        m_Line.numPositions = m_NumberOfPoints;
	}
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i < m_NumberOfPoints; i++)
        {
            float norm = (float)i / (float)(m_NumberOfPoints-1);
           

            Vector3 pos = transform.position;
            pos.x += -(m_Length / 2f) + (norm * m_Length);
            pos.y += -(m_DisplayAmp / 2f) + (m_LFO.GetValueWithOffset(norm * m_DisplayFreq, true) * m_DisplayAmp);

            m_Line.SetPosition(i, pos);
        }        
    }

    void SetWaveform(int index)
    {
        m_LFO.SetWaveform(index);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            string[] testMenu = new string[] 
            { 
                "Sin",
		        "Cos",
		        "Tan",
		        "Sqrt",
		        "Sqr",
		        "SawUp",
		        "SawDown",
		        "Square" 
            };

            m_RadialMenu.Reset();
            m_RadialMenu.m_MenuName = "LFO";
            m_RadialMenu.AddList("Waves", gameObject, "SetWaveform", testMenu);
            m_RadialMenu.AddSlider("Freq", m_LFO.gameObject, "SetFrequency", -2f, 2, m_LFO.m_Frequency );
            m_RadialMenu.AddSlider("Phase", m_LFO.gameObject, "SetPhase", -1f, 1, m_LFO.m_Phase);

            m_RadialMenu.SetPosition(Input.mousePosition);
            m_RadialMenu.ActivateMenu();
        }
    }

    void OnDrawGizmos()
    {
        Vector3 prevPos = Vector3.zero;

        prevPos = transform.position;
        prevPos.x += -(m_Length / 2f) + (0 * m_Length);
        prevPos.y += -(m_DisplayAmp / 2f) + (m_LFO.GetValueWithOffset(0 * m_DisplayFreq, true) * m_DisplayAmp);
        
        for (int i = 0; i < m_NumberOfPoints; i++)
        {
            float norm = (float)i / (float)(m_NumberOfPoints - 1);

            Vector3 pos = transform.position;
            pos.x += -(m_Length / 2f) + (norm * m_Length);
            pos.y += -(m_DisplayAmp / 2f) + (m_LFO.GetValueWithOffset(norm * m_DisplayFreq, true) * m_DisplayAmp);

            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
        }
    }
}
