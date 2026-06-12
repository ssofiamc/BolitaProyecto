using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("Asignar el GameObject de la bolita aquí.")]
    public Transform objetivo; // La cámara seguirá a este transform (la bolita).

    [Header("Configuración")]
    [Tooltip("Altura fija de la cámara en el eje Y del mundo.")]
    public float alturaFija = 12f; // Altura constante sobre el suelo

    [Tooltip("Tiempo de suavizado. Más bajo = más rápida. 0 = inmediata.")]
    [Range(0f, 0.5f)]
    public float suavizado = 0.08f; // Tiempo para alcanzar la posición deseada (en segundos).

    [Tooltip("Offset lateral/frontal de la cámara respecto a la bolita.")]
    public Vector3 offset = Vector3.zero; // Desplazamiento adicional desde la posición de la bolita (en XZ).

    private Vector3 velocidadActual = Vector3.zero; // Variable interna para SmoothDamp (no se asigna manualmente).

    private void LateUpdate() // Se llama después de que todos los objetos hayan actualizado su posición
    {
        if (objetivo == null) // Verificar que se haya asignado un objetivo
        {
            Debug.LogWarning("[FollowCamera] No hay objetivo asignado."); // Evitar errores si no se asigna la bolita
            return;
        }

        Vector3 posicionDeseada = new Vector3( // Calcular la posición deseada de la cámara
            objetivo.position.x + offset.x, // Mantener la misma posición X que la bolita + offset
            alturaFija, // Altura fija en Y
            objetivo.position.z + offset.z - 5f // Mantener la misma posición Z que la bolita + offset (con un pequeño retroceso para mejor vista)
        );

        if (suavizado <= 0f) // Si el suavizado es 0 o negativo, mover la cámara directamente sin suavizado
        {
            transform.position = posicionDeseada; // Mover la cámara directamente a la posición deseada
        }
        else
        {
            transform.position = Vector3.SmoothDamp( // Suavizar el movimiento de la cámara hacia la posición deseada
                transform.position, // Posición actual de la cámara
                posicionDeseada, // Posición objetivo a alcanzar
                ref velocidadActual, // Variable de velocidad que SmoothDamp actualiza internamente (se pasa por referencia)
                suavizado // Tiempo de suavizado para alcanzar la posición deseada (en segundos)
            );
        }
    }
}