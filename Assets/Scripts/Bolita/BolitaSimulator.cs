using UnityEngine;
using System;

public class BolitaSimulator : MonoBehaviour
{
    public static BolitaSimulator Instancia { get; private set; }

    [Header("Configuración de Simulación")]
    [Tooltip("Tamaño del paso de tiempo fijo Δt (segundos). 0.02 = 50Hz.")]
    [Min(0.0001f)] public float updateTime = 0.01f;

    [Tooltip("Escala de tiempo de simulación (1 = tiempo real, 0.5 = cámara lenta).")]
    [Range(0f, 5f)] public float escalaSimulacion = 1f;

    [Tooltip("¿La simulación está pausada?")]
    public bool estaPausada = false;

    // Estado interno
    private float timerAcumulado = 0f;

    // Información pública (solo lectura)
    public float TiempoSimulacion { get; private set; } = 0f;
    public int ContadorPasos { get; private set; } = 0;

    // ─────────────────────────────────────────────
    // EVENTOS
    // ─────────────────────────────────────────────
    /// <summary>Se dispara una vez por paso de simulación. Recibe el dt.</summary>
    public event Action<float> OnPaso;

    /// <summary>Se dispara cuando el simulador recibe la orden de reset.</summary>
    public event Action OnReset;

    // ─────────────────────────────────────────────
    // CICLO DE VIDA
    // ─────────────────────────────────────────────
    private void Awake()
    {
        // Singleton
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (estaPausada) 
            return;

        // Acumular tiempo real escalado
        timerAcumulado += Time.deltaTime * escalaSimulacion;

        // Disparar pasos de simulación hasta consumir el tiempo acumulado
        // (máximo 5 pasos por frame para evitar "death spiral")
        int pasos = 0;
        while (timerAcumulado >= updateTime && pasos < 5)
        {
            EjecutarPaso();
            timerAcumulado -= updateTime;
            pasos++;
        }
    }

    private void EjecutarPaso()
    {
        TiempoSimulacion += updateTime;
        ContadorPasos++;
        OnPaso?.Invoke(updateTime);
    }

    // ─────────────────────────────────────────────
    // CONTROLES PÚBLICOS
    // ─────────────────────────────────────────────
    public void Play() => estaPausada = false;

    public void Pausar() => estaPausada = true;

    public void AlternarPausa() => estaPausada = !estaPausada;

    public void Resetear()
    {
        timerAcumulado = 0f;
        TiempoSimulacion = 0f;
        ContadorPasos = 0;
        OnReset?.Invoke();
    }
}
