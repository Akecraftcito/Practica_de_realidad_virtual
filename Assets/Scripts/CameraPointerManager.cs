using UnityEngine;
using UnityEngine.EventSystems;

// Version ampliada de CameraPointerManager.cs para esta sesion: se agrega un patron
// singleton (Instance) para que otros scripts (UIElementXR, GazeManager) puedan acceder
// a este componente facilmente, y un campo publico hitPoint con el punto exacto donde el
// rayo de la mirada esta chocando, que UIElementXR usa para ubicar los clics de UI.
public class CameraPointerManager : MonoBehaviour
{
    public static CameraPointerManager Instance;

    [SerializeField] private GameObject pointer;
    [SerializeField] private float maxDistancePointer = 4.5f;
    [Range(0, 1)]
    [SerializeField] private float disPointerObject = 0.95f;

    private const float _maxDistance = 10;
    private GameObject _gazedAtObject = null;

    private readonly string interactableTag = "Interactable";
    private float scaleSize = 0.025f;

    [HideInInspector]
    public Vector3 hitPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this) //Con esta funcion nos aseguramos que solo exista una instancia de CameraPointerManager
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
        {
            hitPoint = hit.point; //Asignar el Point a la variable publica hitPoint

            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                _gazedAtObject?.SendMessage("OnPointerExitXR", null, SendMessageOptions.DontRequireReceiver);
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnterXR", null, SendMessageOptions.DontRequireReceiver);
                GazeManager.Instance.StartGazeSelection(); //Se habilita la Barra de carga
            }

            if (hit.transform.CompareTag(interactableTag)) //comparamos si el objeto tiene el tag que hemos definido
            {
                PointerOnGaze(hit.point); //Funcion para mostrar el pointer
            }
            else
            {
                PointerOutGaze(); //Funcion para retirar el puntero
            }
        }
        else
        {
            _gazedAtObject?.SendMessage("OnPointerExitXR", null, SendMessageOptions.DontRequireReceiver);
            _gazedAtObject = null;
            PointerOutGaze();
        }
    }

    private void PointerOnGaze(Vector3 point)
    {
        pointer.SetActive(true);
        float scaleFactor = scaleSize * Vector3.Distance(transform.position, point);
        pointer.transform.localScale = Vector3.one * scaleFactor;
        pointer.transform.position = CalculatePointerPosition(transform.position, point, disPointerObject);
    }

    private void PointerOutGaze()
    {
        pointer.SetActive(false);
    }

    // Interpolacion lineal entre 2 puntos: L(x,y,z) = p0 + t * (p1 - p0)
    private Vector3 CalculatePointerPosition(Vector3 p0, Vector3 p1, float t)
    {
        float x = p0.x + t * (p1.x - p0.x);
        float y = p0.y + t * (p1.y - p0.y);
        float z = p0.z + t * (p1.z - p0.z);
        return new Vector3(x, y, z);
    }
}