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
        pos = new Vector3(-270.0f, pos.y);
        goArrow.transform.position = this.transform.position;
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
