
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;

    [Header("Altura")]
    public float alturaFija = 10f;

    [Header("Suavizado")]
    [Range(0f, 1f)]
    public float suavizado = 0.1f;


    public Vector3 velocidadActual = Vector3.zero;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("Falta asignar el objetivo");
            return;
        }

        Vector3 posicionDeseada = new Vector3(objetivo.position.x, alturaFija, objetivo.position.z);

        if (suavizado <= 0f)
        {
            transform.position = posicionDeseada;
        }
        else
        {
            transform .position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidadActual, suavizado);
        }
    }
}