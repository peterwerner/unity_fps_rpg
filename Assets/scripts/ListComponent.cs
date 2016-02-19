using UnityEngine;
using System.Collections.Generic;

/**
 * http://garry.tv/2015/06/14/unity-tips/
 */
public abstract class ListComponent<T> : MonoBehaviour where T : MonoBehaviour
{
	public static List<T> InstanceList = new List<T>();

	protected virtual void OnEnable()
	{
		InstanceList.Add( this as T );
	}

	protected virtual void OnDisable()
	{
		InstanceList.Remove( this as T );
	}

}