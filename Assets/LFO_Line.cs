using UnityEngine;
using System.Collections;

public class LFO_Line : MonoBehaviour
{
    LineRenderer m_Line;
    int m_NumberOfPoints = 50;

    public ParticleSystem m_PSys;

    public LFO m_LFO;
    public float m_Length;
    public float m_DisplayFreq = 1;

    public float m_DisplayAmp = 1;

    public RadialMenu m_RadialMenu;

	// Use this for initialization
	void Start () 
    {
        m_Line = GetComponent<LineRenderer>();
        m_Line.SetVertexCount(m_NumberOfPoints);

        for (int i = 0; i < m_NumberOfPoints; i++)
        {
            m_PSys.Emit(Vector3.zero, Vector3.zero, .1f, float.MaxValue, Color.white);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[m_NumberOfPoints];
        m_PSys.GetParticles(particles);

        for (int i = 0; i < m_NumberOfPoints; i++)
        {
            float norm = (float)i/(float)m_NumberOfPoints;

            Vector3 pos = transform.position;
            pos.x += -(m_Length / 2f) + (norm * m_Length);
            pos.y += -(m_DisplayAmp / 2f) + (m_LFO.GetValueWithOffset( norm * m_DisplayFreq ) * m_DisplayAmp);

            particles[i].position = pos;
        }
        m_PSys.SetParticles(particles, m_NumberOfPoints);

        
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
}
