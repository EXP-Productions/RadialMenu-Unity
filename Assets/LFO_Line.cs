using UnityEngine;
using System.Collections;

public class LFO_Line : MonoBehaviour
{
    LineRenderer m_Line;
    int m_NumberOfPoints = 300;

    public ParticleSystem m_PSys;

    public LFO m_LFO;
    public float m_Length;
    public float m_DisplayFreq = 1;

    public float m_DisplayAmp = 1;

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
            pos.y += -(m_DisplayAmp / 2f) + (m_LFO.GetValueWithOffset(norm * m_DisplayFreq) * m_DisplayAmp);

            particles[i].position = pos;
        }
        m_PSys.SetParticles(particles, m_NumberOfPoints);

        

        for (int i = 0; i < m_NumberOfPoints; i++)
        {
            float norm = (float)i/(float)m_NumberOfPoints;
            Vector3 pos = transform.position;
            pos.x += -(m_Length / 2f) + (norm * m_Length);
            pos.y += -(m_DisplayAmp / 2f) + (m_LFO.GetValueWithOffset(norm * m_DisplayFreq) * m_DisplayAmp);
            m_Line.SetPosition(i, pos);
        }
	}
}
