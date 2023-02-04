using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShadeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // Unity variables
    private Image image;
    private Button button;

    // Inspector variables
    [SerializeField] Sprite baseTexture;
    [SerializeField] Sprite hoverTexture;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        image = GetComponent<Image>();
        image.sprite = baseTexture;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            image.sprite = hoverTexture;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
            image.sprite = baseTexture;
    }
}
