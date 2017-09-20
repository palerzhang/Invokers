using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	private Vector3 offset = new Vector3(0,0,0);
	private GameObject preHit = null;

	public void OnDrag(PointerEventData eventData)
	{
		// eventData.position.xy == Input.mousePosition.xy
		transform.position=Input.mousePosition + offset;
		GameObject hit = GetHitObject ();
		if (hit != null) 
		{
			if (hit.tag == "Droppable") 
			{
				Debug.Log ("Hit!");
				if (hit != preHit) 
				{
					Debug.Log ("First Hit");
					Debug.Log (preHit);
					hit.GetComponent<Image> ().color = new Color (0.0f, 1.0f, 0.0f, 0.5f);
					preHit.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
					preHit = hit;
				}
			}
		} 
		else 
		{
			if (preHit != null) 
			{
				preHit.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				preHit = null;
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		//when pressed
		offset = transform.position - Input.mousePosition;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		//when released
		//GameObject hit = GetHitObject();
		//if (hit != null)
		//{
		//	if (hit.tag == "Droppable")
		//	{
		//		Debug.Log ("First Drop");
		//		Debug.Log (preHit);
		//		hit.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
		//		transform.position = hit.transform.position;
		//		transform.parent = hit.transform;
		//	}
		//}

		RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition,-Vector2.up);
		Debug.Log (hit.collider);
		if (hit.collider != null) {
			//如果射线检测到的gameobject为grid，就把当前Panel放在grid节点下
			if(hit.collider.gameObject.tag == "Droppable")
				Debug.Log ("First Drop");
		}
	}

	GameObject GetHitObject()
	{
		Collider2D hit = Physics2D.Raycast (Input.mousePosition, -Vector2.up).collider;
		//Debug.Log (hit);
		if (hit == null)
			return null;
		return hit.gameObject;
	}

	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	void Update () {

	}
}
