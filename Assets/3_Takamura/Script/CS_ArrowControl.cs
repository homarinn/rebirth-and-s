using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CS_ArrowControl : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField, Header("–îˆó")]
    GameObject goArrow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var pos = this.transform.position;
        pos = new Vector3(-250.0f, pos.y);
        Debug.Log(pos);
        Debug.Log(goArrow.transform.position);
        goArrow.transform.position = this.transform.position;
        Debug.Log(goArrow.transform.position);
        goArrow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        goArrow.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        goArrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
