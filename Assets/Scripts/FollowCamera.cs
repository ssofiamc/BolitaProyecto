using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("Asignar el GameObject de la bolita aquí.")]
    public Transform objetivo;

    [Header("Configuración")]
    [Tooltip("Altura fija de la cámara en el eje Y del mundo.")]
    public float alturaFija = 12f;

    [Tooltip("Tiempo de suavizado. Más bajo = más rápida. 0 = inmediata.")]
    [Range(0f, 0.5f)]
    public float suavizado = 0.08f;

    [Tooltip("Offset lateral/frontal de la cámara respecto a la bolita.")]
    public Vector3 offset = Vector3.zero;

    // ─────────────────────────────────────────────
    // Estado interno del SmoothDamp
    // ─────────────────────────────────────────────
    private Vector3 velocidadActual = Vector3.zero;

    // ─────────────────────────────────────────────
    // LateUpdate: se ejecuta DESPUÉS de todos los Update.
    // Ideal para cámara: la bolita ya se movió este frame.
    // ─────────────────────────────────────────────
    private void LateUpdate()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("[FollowCamera] No hay objetivo asignado.");
            return;
        }

        // Posición deseada: misma XZ que la bolita + altura fija + offset
        Vector3 posicionDeseada = new Vector3(
            objetivo.position.x + offset.x,
            alturaFija,
            objetivo.position.z + offset.z
        );

        if (suavizado <= 0f)
        {
            transform.position = posicionDeseada;
        }
        else
        {
            // SmoothDamp: movimiento suave con aceleración/desaceleración
            transform.position = Vector3.SmoothDamp(
                transform.position,
                posicionDeseada,
                ref velocidadActual,
                suavizado
            );
        }
    }
}