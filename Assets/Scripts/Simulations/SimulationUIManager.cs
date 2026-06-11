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

    private void Start()
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

    private void CambiarMagnetismoFuerza(float valor)
    {
        foreach (MagneticSimulation sim in FindObjectsOfType<MagneticSimulation>())
        {
            sim.constanteK = valor;
        }
    }

    private void CambiarMagnetismoRadio(float valor)
    {
        foreach (MagneticSimulation sim in FindObjectsOfType<MagneticSimulation>())
        {
            sim.radioAccion = valor;
        }
    }

    // =====================
    // VIENTO
    // =====================

    private void CambiarVientoIntensidad(float valor)
    {
        foreach (WindSimulation sim in FindObjectsOfType<WindSimulation>())
        {
            sim.intensidadBase = valor;
        }
    }

    private void CambiarVientoOscilante(bool valor)
    {
        foreach (WindSimulation sim in FindObjectsOfType<WindSimulation>())
        {
            sim.esOscilante = valor;
        }
    }

    // =====================
    // SALTO
    // =====================

    private void CambiarSaltoImpulso(float valor)
    {
        foreach (JumpSimulation sim in FindObjectsOfType<JumpSimulation>())
        {
            sim.impulsoVertical = valor;
        }
    }

    private void CambiarSaltoRecarga(float valor)
    {
        foreach (JumpSimulation sim in FindObjectsOfType<JumpSimulation>())
        {
            sim.tiempoRecarga = valor;
        }
    }

    // =====================
    // BOOST
    // =====================

    private void CambiarBoostFuerza(float valor)
    {
        foreach (BoostSimulation sim in FindObjectsOfType<BoostSimulation>())
        {
            sim.fuerzaImpulso = valor;
        }
    }

    private void CambiarBoostRecarga(float valor)
    {
        foreach (BoostSimulation sim in FindObjectsOfType<BoostSimulation>())
        {
            sim.tiempoRecarga = valor;
        }
    }

    // =====================
    // SUPERFICIES
    // =====================

    private void CambiarSuperficieA(float valor)
    {
        foreach (SurfaceSimulation sim in FindObjectsOfType<SurfaceSimulation>())
        {
            sim.friccionA = valor;
        }
    }

    private void CambiarSuperficieB(float valor)
    {
        foreach (SurfaceSimulation sim in FindObjectsOfType<SurfaceSimulation>())
        {
            sim.friccionB = valor;
        }
    }

    private void CambiarSuperficieC(float valor)
    {
        foreach (SurfaceSimulation sim in FindObjectsOfType<SurfaceSimulation>())
        {
            sim.friccionC = valor;
        }
    }
}