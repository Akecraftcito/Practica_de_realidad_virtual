// ===================== Player/Main Camera/CameraPointerManager.cs =====================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// REQUISITO: este script va en el GameObject "Main Camera" que esta dentro de "Player".
// Lanza el Raycast desde la camara; cuando el rayo choca con un objeto Tag=Interactable,
// avisa a GazeManager para que empiece a contar el tiempo de seleccion (la "barra de carga").
public class CameraPointerManager : MonoBehaviour
{
    public static CameraPointerManager instance;
    [SerializeField] private GameObject pointer;
    [SerializeField] private float maxDistancePointer = 15f;
    private readonly string interactableTag = "Interactable";
    private float scaleSize = 0.025f;
    [Range(0, 1)]
    [SerializeField] private float distPointerObject = 0.95f;

    private const float _maxDistance = 15;
    private GameObject _gazedAtObject = null;

    [HideInInspector]
    public Vector3 hitPoint;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }

    private void Start()
    {
        // Nos suscribimos al evento de GazeManager: cuando se completa el tiempo de mirada,
        // GazeSelection() le avisa al objeto mirado que "hicieron clic" en el.
        GazeManager.Instance.OnGazeSelection += GazeSelection;
    }

    private void GazeSelection() {
        _gazedAtObject?.SendMessage("OnPointerClickXR", null, SendMessageOptions.DontRequireReceiver);
    }

    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
        {
            hitPoint = hit.point;

            if (_gazedAtObject != hit.transform.gameObject)
            {
                if (_gazedAtObject != null)
                { _gazedAtObject?.SendMessage("OnPointerExitXR", null, SendMessageOptions.DontRequireReceiver); }
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnterXR", null, SendMessageOptions.DontRequireReceiver);
                GazeManager.Instance.StartGazeSelection();
            }
            if (hit.transform.CompareTag(interactableTag))
            {
                PointerOnGaze(hit.point);
            }
            else {
                PointerOutGaze();
            }
        }
        else
        {
            if (_gazedAtObject != null)
            { _gazedAtObject?.SendMessage("OnPointerExitXR", null, SendMessageOptions.DontRequireReceiver); }
            _gazedAtObject = null;
            PointerOutGaze();
        }

        // Checks for screen touches.
        if (Google.XR.Cardboard.Api.IsTriggerPressed)
        {
            _gazedAtObject?.SendMessage("OnPointerClickXR", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void PointerOutGaze() {
        if (pointer != null)
        {
            pointer.transform.localScale = Vector3.one * 0.1f;

            if (pointer.transform.parent != null)
            {
                pointer.transform.parent.localPosition = new Vector3(0, 0, maxDistancePointer);
            }

            if (pointer.transform.parent != null && pointer.transform.parent.parent != null)
            {
                pointer.transform.parent.parent.rotation = transform.rotation;
            }
        }

        if (GazeManager.Instance != null)
        {
            GazeManager.Instance.CancelGazeSelection();
        }
    }

    private void PointerOnGaze(Vector3 hitPoint) {
        if (pointer == null)
        {
            return;
        }

        float scaleFactor = scaleSize * Vector3.Distance(transform.position, hitPoint);
        pointer.transform.localScale = Vector3.one * scaleFactor;

        if (pointer.transform.parent != null)
        {
            pointer.transform.parent.position = CalculatePointerPosition(transform.position, hitPoint, distPointerObject);
        }
    }
    private Vector3 CalculatePointerPosition(Vector3 p0, Vector3 p1, float t) {
        float x = p0.x + t * (p1.x - p0.x);
        float y = p0.y + t * (p1.y - p0.y);
        float z = p0.z + t * (p1.z - p0.z);
        return new Vector3(x, y, z);
    }
}

