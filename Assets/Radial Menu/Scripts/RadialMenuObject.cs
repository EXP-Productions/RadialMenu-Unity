using UnityEngine;
using System.Collections;

public class RadialMenuObject : MonoBehaviour 
{
    public string       m_ObjectName;
    public GameObject   m_ObjectToCall;
    public string       m_FunctionToCall;

    public virtual void Init( string name, GameObject objectToCall, string functionToCall )
    {
        m_ObjectName = name;
        m_ObjectToCall = objectToCall;
        m_FunctionToCall = functionToCall;
    }
	
    public virtual void CallFunction()
    {
        // do what ever the object does here
    }
}
