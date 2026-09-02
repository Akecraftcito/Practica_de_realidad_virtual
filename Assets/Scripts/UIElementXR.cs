using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIElementXR : MonoBehaviour
{
    public UnityEvent OnXRPointerEnter;
    public UnityEvent OnXRPointerExit;
    private Camera xRCamera;

    // Start is called before the first frame update
    void Start()
    {
        xRCamera = CameraPointerManager.Instance.gameObject.GetComponent<Camera>(); //Llamamos aqui a la clase CameraPointerManager como instance en UIElementXR;
    }

    // Update is called once per frame
    public void OnPointerClicXR()
    {
        PointerEventData pointerEvent = PlacePointer();//Este es el elemento que nos permitira hacer clic sobre el elemento UI pero para ello se necesita una posicion la cual esta en la funcion PlacePointer
        ExecuteEvents.Execute(this.gameObject, pointerEvent, ExecuteEvents.pointerClickHandler); //Ejecutamos el Evento al hacer clic;
    }

    public void OnPointerEnterXR()
    {
        GazeManager.Instance.SetUpGaze(1.5f); //Reducimos el tiempo de carga del evento;
        OnXRPointerEnter?.Invoke(); //Llamaremos al EventoEnterXR si no hay problemas..

        PointerEventData pointerEvent = PlacePointer();//Este es el elemento que nos permitira hacer clic sobre el elemento UI pero para ello se necesita una posicion la cual esta en la funcion PlacePointer
        ExecuteEvents.Execute(this.gameObject, pointerEvent, ExecuteEvents.pointerDownHandler); //Ejecutamos el Evento al hacer clic;
    }

    public void OnPointerExitXR()
    {
        GazeManager.Instance.SetUpGaze(2.5f); //Reducimos el tiempo de carga del evento;
        OnXRPointerExit?.Invoke(); //Llamaremos al EventoEnterXR si no hay problemas..

        PointerEventData pointerEvent = PlacePointer();//Este es el elemento que nos permitira hacer clic sobre el elemento UI pero para ello se necesita una posicion la cual esta en la funcion PlacePointer
        ExecuteEvents.Execute(this.gameObject, pointerEvent, ExecuteEvents.pointerUpHandler); //Ejecutamos el Evento al hacer clic;
    }

    public PointerEventData PlacePointer()
    {
        Vector3 screePos = xRCamera.WorldToScreenPoint(CameraPointerManager.Instance.hitPoint);
        var pointer = new PointerEventData(EventSystem.current);
        pointer.position = new Vector2(screePos.x, screePos.y);
        return pointer;
    }
}