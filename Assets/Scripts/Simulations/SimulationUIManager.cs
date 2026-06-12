using UnityEngine;
using UnityEngine.UI;

public class SimulationUIManager : MonoBehaviour
{
    [Header("Magnetismo")]
    public Slider sliderMagnetismoFuerza;
    public Slider sliderMagnetismoRadio;

    [Header("Viento")]
    public Slider sliderVientoIntensidad;
    public Toggle toggleVientoOscilante;

    [Header("Salto")]
    public Slider sliderSaltoImpulso;
    public Slider sliderSaltoRecarga;

    [Header("Boost")]
    public Slider sliderBoostFuerza;
    public Slider sliderBoostRecarga;

    [Header("Superficies")]
    public Slider sliderSuperficieA;
    public Slider sliderSuperficieB;
    public Slider sliderSuperficieC;

    private void Start() // Suscribir eventos de UI
    {
        // Magnetismo
        sliderMagnetismoFuerza.onValueChanged.AddListener(CambiarMagnetismoFuerza);
        sliderMagnetismoRadio.onValueChanged.AddListener(CambiarMagnetismoRadio);

        // Viento
        sliderVientoIntensidad.onValueChanged.AddListener(CambiarVientoIntensidad);
        toggleVientoOscilante.onValueChanged.AddListener(CambiarVientoOscilante);

        // Salto
        sliderSaltoImpulso.onValueChanged.AddListener(CambiarSaltoImpulso);
        sliderSaltoRecarga.onValueChanged.AddListener(CambiarSaltoRecarga);

        // Boost
        sliderBoostFuerza.onValueChanged.AddListener(CambiarBoostFuerza);
        sliderBoostRecarga.onValueChanged.AddListener(CambiarBoostRecarga);

        // Superficies
        sliderSuperficieA.onValueChanged.AddListener(CambiarSuperficieA);
        sliderSuperficieB.onValueChanged.AddListener(CambiarSuperficieB);
        sliderSuperficieC.onValueChanged.AddListener(CambiarSuperficieC);
    }

    // =====================
    // MAGNETISMO
    // =====================

    private void CambiarMagnetismoFuerza(float valor) // Cambiar la fuerza de magnetismo (constante K)
    {
        foreach (MagneticSimulation sim in FindObjectsOfType<MagneticSimulation>()) // Actualizar todas las simulaciones de magnetismo
        {
            sim.constanteK = valor; // Cambiar la constante K en cada simulación
        }
    }

    private void CambiarMagnetismoRadio(float valor) // Cambiar el radio de acción del magnetismo
    {
        foreach (MagneticSimulation sim in FindObjectsOfType<MagneticSimulation>()) //  Actualizar todas las simulaciones de magnetismo
        {
            sim.radioAccion = valor; // Cambiar el radio de acción en cada simulación
        }
    }

    // =====================
    // VIENTO
    // =====================

    private void CambiarVientoIntensidad(float valor) // Cambiar la intensidad base del viento
    {
        foreach (WindSimulation sim in FindObjectsOfType<WindSimulation>()) // Actualizar todas las simulaciones de viento
        {
            sim.intensidadBase = valor; // Cambiar la intensidad base en cada simulación
        }
    }

    private void CambiarVientoOscilante(bool valor) // Cambiar si el viento es oscilante o constante
    {
        foreach (WindSimulation sim in FindObjectsOfType<WindSimulation>()) // Actualizar todas las simulaciones de viento
        {
            sim.esOscilante = valor; // Cambiar el tipo de viento en cada simulación
        }
    }

    // =====================
    // SALTO
    // =====================

    private void CambiarSaltoImpulso(float valor) // Cambiar la fuerza de impulso del salto
    {
        foreach (JumpSimulation sim in FindObjectsOfType<JumpSimulation>()) // Actualizar todas las simulaciones de salto
        {
            sim.impulsoVertical = valor; // Cambiar la fuerza de impulso en cada simulación
        }
    }

    private void CambiarSaltoRecarga(float valor) // Cambiar el tiempo de recarga del salto
    {
        foreach (JumpSimulation sim in FindObjectsOfType<JumpSimulation>()) // Actualizar todas las simulaciones de salto
        {
            sim.tiempoRecarga = valor; // Cambiar el tiempo de recarga en cada simulación
        }
    }

    // =====================
    // BOOST
    // =====================

    private void CambiarBoostFuerza(float valor) // Cambiar la fuerza de impulso del boost
    {
        foreach (BoostSimulation sim in FindObjectsOfType<BoostSimulation>()) // Actualizar todas las simulaciones de boost
        {
            sim.fuerzaImpulso = valor; // Cambiar la fuerza de impulso en cada simulación
        }
    }

    private void CambiarBoostRecarga(float valor) // Cambiar el tiempo de recarga del boost
    {
        foreach (BoostSimulation sim in FindObjectsOfType<BoostSimulation>()) // Actualizar todas las simulaciones de boost
        {
            sim.tiempoRecarga = valor; // Cambiar el tiempo de recarga en cada simulación
        }
    }

    // =====================
    // SUPERFICIES
    // =====================

    private void CambiarSuperficieA(float valor) // Cambiar la fricción de la superficie A
    {
        foreach (SurfaceSimulation sim in FindObjectsOfType<SurfaceSimulation>()) // Actualizar todas las simulaciones de superficies
        {
            sim.friccionA = valor; // Cambiar la fricción de la superficie A en cada simulación
        }
    }

    private void CambiarSuperficieB(float valor) // Cambiar la fricción de la superficie B
    {
        foreach (SurfaceSimulation sim in FindObjectsOfType<SurfaceSimulation>()) // Actualizar todas las simulaciones de superficies
        {
            sim.friccionB = valor; // Cambiar la fricción de la superficie B en cada simulación
        }
    }

    private void CambiarSuperficieC(float valor) // Cambiar la fricción de la superficie C
    {
        foreach (SurfaceSimulation sim in FindObjectsOfType<SurfaceSimulation>()) // Actualizar todas las simulaciones de superficies
        {
            sim.friccionC = valor; // Cambiar la fricción de la superficie C en cada simulación
        }
    }
}