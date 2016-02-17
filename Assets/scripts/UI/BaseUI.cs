using UnityEngine;
using System.Collections;

public class BaseUI : MonoBehaviour {

	[SerializeField] protected GUISkin GUIskin;
	public string displayName;

	public virtual void UpdateDimensions() {}

	public virtual void Hide() {}

	public virtual void Show() {}

}
