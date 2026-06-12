using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("Asignar el GameObject de la bolita aquí.")]
    public Transform objetivo;

    [Header("Configuración")]
    [Tooltip("Altura adicional sobre el objetivo.")]
    public float alturaFija = 12f;

    [Tooltip("Tiempo de suavizado. Más bajo = más rápida. 0 = inmediata.")]
    [Range(0f, 0.5f)]
    public float suavizado = 0.08f;

    [Tooltip("Offset lateral/frontal de la cámara respecto a la bolita.")]
    public Vector3 offset = Vector3.zero;

    private Vector3 velocidadActual = Vector3.zero;

    private void LateUpdate()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("[FollowCamera] No hay objetivo asignado.");
            return;
        }

        Vector3 posicionDeseada = new Vector3(
            objetivo.position.x + offset.x,
            objetivo.position.y + alturaFija,
            objetivo.position.z + offset.z - 5f
        );

        if (suavizado <= 0f)
        {
            transform.position = posicionDeseada;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                posicionDeseada,
                ref velocidadActual,
                suavizado
            );
        }
    }
}