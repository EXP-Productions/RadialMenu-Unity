using UnityEngine;
using System.Collections;

public class LFO : MonoBehaviour
{
	
	public bool 	m_LinkedWithMasterSpeedController = false;
    	
	public Wave.Waveform m_Waveform = Wave.Waveform.Sin;
		
	public float 	m_ContinuousValue;
	public float 	ContinuousValue{ get{ return m_ContinuousValue; } }

    public float CurrentAngle { get { return ((m_PositionInCycle + m_Phase )* 360); } }	// Each cycle goes through 360 degrees
	protected float m_PositionInCycle;		// Normalized position in cycle
	protected float m_PrevPositionInCycle;	// Previous normalized position in cycle
	public float 	m_Frequency = 1;		// Cycles per second
	public float 	m_Amplitude = 1;		// Height of the peaks
    public float    m_Phase = 0;
	float 			m_FrequencyScaler = 1;	// For controlling the speed, reversing, fast forwarding and setting divisions of a master beat 16ths 8ths
	public 	float	SpeedMultiplyer { get{ return m_FrequencyScaler; } }
	public float 	m_CurrentNormaliedValue;
	float 			m_OutputValue { get{ return m_CurrentNormaliedValue * m_Amplitude; } }
		
	// Speed controls
	public bool 	m_DrawFrequencySlider = false;
	public bool 	m_Paused = false;

  

   
	
	void Start()
	{
		//BPMCounter.onSetBPM += onSetBPM; // needs to be replaced by a master timer
	}
	
	void SetFreqFromBPM( float bpm )
	{
		m_Frequency = bpm / 60;
	}
	
	public float GetValueWithOffset( float offset )
	{
        return Mathf.Clamp01( Wave.Evaluate(m_Waveform, m_PositionInCycle + m_Phase + offset) );

        /*
		float currentAmplitude = Mathf.Sin( (CurrentAngle * Mathf.Deg2Rad) + ((offset * 360 * Mathf.Deg2Rad ) * m_Frequency ) );			
		float amp =  (((currentAmplitude + 1) * .5f)  * m_Amplitude);
		//return Mathf.Clamp( amp, 0, m_Amplitude );
		return amp;
         * */
	}
	
	public void SetSpeedMultiplyer( float speedMultiplyer  )
	{
		m_FrequencyScaler = speedMultiplyer;
	}
	
	public void Update()
	{
		if( m_Waveform == Wave.Waveform.None ) return;
		// Update the normalized position by delta time and normalize it to 0-1
		// * Could go before or after pause depending on desired effect *
		
		// Check for input controls 
		//if( Input.GetKey( InputManager.m_ReverseLFO ) ) m_Reverse = true;
		//else m_Reverse = false;
		
		//if( Input.GetKey( InputManager.m_FastforwardLFO ) ) m_DoubleSpeed = true;
		//else m_DoubleSpeed = false;
		
		if( m_Paused ) return;
		
		// Get time since last frame
		float time = Time.deltaTime * m_FrequencyScaler;
		
		//// Adjust time by speed multiplyer. Usefull for fast forwarding and reversing
		//time *= m_SpeedMultiplyer;
		
		// Update normalized and continuous values
		m_PositionInCycle += m_Frequency * time;
        m_PositionInCycle %= 1;
		
		// Update continuous value
		m_ContinuousValue += m_Frequency * time;
		
		 		/*
		// Update output value depending on the waveform
		if( m_Waveform == Wave.Waveform.Sin )
		{			
			float currentAmplitude = Mathf.Sin( CurrentAngle * Mathf.Deg2Rad );			
			m_CurrentNormaliedValue = ( currentAmplitude + 1 ) * .5f; // Normalizes the current amplitude to 0 - 1
		}
		else if( m_Waveform == Wave.Waveform.SawUp )
		{						
			m_CurrentNormaliedValue = m_PositionInCycle;
		}
		else if( m_Waveform == Wave.Waveform.SawDown )
		{			
			m_CurrentNormaliedValue = 1 - m_PositionInCycle;
		}
		else if( m_Waveform == Wave.Waveform.Square )
		{			
			m_CurrentNormaliedValue = 0;
			if( m_PositionInCycle > .5f ) m_CurrentNormaliedValue = 1;
		}
		*/
		
		
		if( m_FrequencyScaler > 0  )
		{			
			if( m_PrevPositionInCycle > m_PositionInCycle )
				Trigger();
		}
		else if( m_PrevPositionInCycle < m_PositionInCycle )
			Trigger();
		
		m_PrevPositionInCycle = m_PositionInCycle;
		
	}	
	
	protected virtual void Trigger()
	{
		
	}

    public void SetFrequency( float freq )
    {
        m_Frequency = freq;
    }

    public void SetPhase(float phase)
    {
        m_Phase = phase;
    }
	
	public void SetWaveform( int wavefromIndex )
	{
		m_Waveform = (Wave.Waveform)wavefromIndex;
		print( m_Waveform );
	}
	
	public void SetWaveform( Wave.Waveform waveform )
	{
		m_Waveform = waveform;
	}

   
	
	void OnDrawGizmos()
	{
		//Gizmos.DrawWireCube( transform.position, new Vector3 
		Gizmos.DrawLine( transform.position,  transform.position + ( Vector3.up * m_CurrentNormaliedValue ) ); 
	}
}


public class Wave
{
	public enum Waveform
	{
		Sin,
		Cos,
		Tan,
		Sqrt,
		Sqr,
		SawUp,
		SawDown,
		Square,
		None,
	}

    public static string[] m_WaveNames = new string[] 
        { 
            "Sin",
		    "Cos",
		    "Tan",
		    "Sqrt",
		    "Sqr",
		    "Saw Up",
		    "Saw Down",
		    "Square" 
        };

	public static float Evaluate( Waveform wave, float normalizedInput )
	{
        normalizedInput = WrapFloatToRange(normalizedInput, 0f, 1f);

		if( wave == Wave.Waveform.Sin )
		{	
			float currentAmplitude = Mathf.Sin( normalizedInput * 360 * Mathf.Deg2Rad );			
			return ( currentAmplitude + 1 ) * .5f; // Normalizes the current amplitude to 0 - 1
		}
		else if( wave == Wave.Waveform.Cos )
		{	
			float currentAmplitude = Mathf.Cos( normalizedInput * 360 * Mathf.Deg2Rad );			
			return ( currentAmplitude + 1 ) * .5f; // Normalizes the current amplitude to 0 - 1
		}
		else if( wave == Wave.Waveform.Tan )
		{	
			float currentAmplitude = Mathf.Tan( normalizedInput * 360 * Mathf.Deg2Rad );			
			return ( currentAmplitude + 1 ) * .5f; // Normalizes the current amplitude to 0 - 1
		}
		else if( wave == Wave.Waveform.Sqrt )
		{			
			return Mathf.Sqrt( normalizedInput );
		}
		else if( wave == Wave.Waveform.Sqr )
		{			
			return normalizedInput * normalizedInput;
		}
		else if( wave == Wave.Waveform.SawUp )
		{						
			return normalizedInput;
		}
		else if( wave == Wave.Waveform.SawDown )
		{			
			return 1 - normalizedInput;
		}
		else if( wave == Wave.Waveform.Square )
		{			
			if( normalizedInput > .5f )
				return 1;
			else
				return 0;
		}

		return 0;
	}

    public static float WrapFloatToRange( float f, float min, float max)
    {
        float result = f;
        float difference = max - min;

        while (result < min || result > max)
        {
            if (result < min)
                result += difference;
            else if (result > max)
                result -= difference;
        }

        return result;
    }
}
