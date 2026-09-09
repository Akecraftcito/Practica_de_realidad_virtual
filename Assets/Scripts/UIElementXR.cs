using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// REQUISITO: este script va en CADA botón con el que se quiera interactuar con la mirada
// (en este ejercicio: los botones "Continuar" y "Salir" del Canvas de pausa). Además, el
// botón necesita Tag=Interactable y un Collider del mismo tamaño que el propio botón.
public class UIElementXR : MonoBehaviour
{
    public UnityEvent OnXRPointerEnter;
    public UnityEvent OnXRPointerExit;
    private Camera xRCamera;

    void Start()
    {
        xRCamera = CameraPointerManager.instance.gameObject.GetComponent<Camera>();
    }

    public void OnPointerClickXR()
    {
        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.Invoke();
            return;
        }

        PointerEventData pointerEvent = PlacePointer();
        ExecuteEvents.Execute(this.gameObject, pointerEvent, ExecuteEvents.pointerClickHandler);
    }

    public void OnPointerEnterXR()
    {
        GazeManager.Instance.SetUpGaze(1.5f);
        OnXRPointerEnter?.Invoke();

        PointerEventData pointerEvent = PlacePointer();
        ExecuteEvents.Execute(this.gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
    }
    public void OnPointerExitXR()
    {
        GazeManager.Instance.SetUpGaze(2.5f);
        OnXRPointerExit?.Invoke();

        PointerEventData pointerEvent = PlacePointer();
        ExecuteEvents.Execute(this.gameObject, pointerEvent, ExecuteEvents.pointerUpHandler);
    }

    public PointerEventData PlacePointer()
    {
        Vector3 screePos = xRCamera.WorldToScreenPoint(CameraPointerManager.instance.hitPoint);
        var pointer = new PointerEventData(EventSystem.current);
        pointer.position = new Vector2(screePos.x, screePos.y);
        return pointer;
    }
}