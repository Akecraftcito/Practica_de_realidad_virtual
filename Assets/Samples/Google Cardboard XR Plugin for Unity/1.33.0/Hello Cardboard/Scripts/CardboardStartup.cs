using Google.XR.Cardboard;
using UnityEngine;

// REQUISITO: este script va en el Prefab del escenario de RV (en este ejemplo, HelloCardboard).
// >>> MUY IMPORTANTE: la linea "Api.UpdateScreenParams()" debe quedar envuelta en
// "#if !UNITY_EDITOR / #endif" (como se ve abajo) para poder usar el Cardboard Simulator
// y probar la RV dentro del Editor de Unity sin que se rompa la ejecucion.
public class CardboardStartup : MonoBehaviour
{
    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }
    }

    public void Update()
    {
        if (Api.IsGearButtonPressed)
        {
            Api.ScanDeviceParams();
        }

        if (Api.IsCloseButtonPressed)
        {
            Application.Quit();
        }

        if (Api.IsTriggerHeldPressed)
        {
            Api.Recenter();
        }

        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
#if !UNITY_EDITOR
        Api.UpdateScreenParams();
#endif
    }
}