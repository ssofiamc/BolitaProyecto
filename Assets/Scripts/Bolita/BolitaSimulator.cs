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

    private float timerAcumulado = 0f; // Acumula el tiempo real escalado para determinar cuándo ejecutar pasos de simulación.

    public float TiempoSimulacion { get; private set; } = 0f; // Tiempo total transcurrido en la simulación (en segundos), incrementado cada paso por updateTime.
    public int ContadorPasos { get; private set; } = 0; // Contador de pasos de simulación ejecutados, incrementado cada vez que se ejecuta un paso.

    public event Action<float> OnPaso; // Dispara el tiempo del paso (Δt) cada vez que se ejecuta un paso de simulación.

    public event Action OnReset; // Dispara un evento de reset cada vez que se llama al método Resetear(), para que otros componentes puedan reiniciar su estado si es necesario.

    private void Awake() // Configuración del singleton
    {
        if (Instancia != null && Instancia != this) // Si ya existe una instancia y no es esta, destruir este objeto para mantener el singleton
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject); // Opcional: Mantener este objeto al cargar nuevas escenas, si se desea que la simulación persista entre escenas.
    }

    private void Update() // Se ejecuta cada frame, maneja la acumulación de tiempo real escalado y ejecuta pasos de simulación según sea necesario.
    {
        if (estaPausada) 
            return; // Si la simulación está pausada, no acumular tiempo ni ejecutar pasos.

        timerAcumulado += Time.deltaTime * escalaSimulacion; // Acumula el tiempo real transcurrido en este frame, escalado por la escala de simulación. Esto permite que la simulación avance más rápido o más lento que el tiempo real según el valor de escalaSimulacion.
        int pasos = 0;
        while (timerAcumulado >= updateTime && pasos < 5) // Mientras el tiempo acumulado sea suficiente para ejecutar al menos un paso de simulación, y para evitar caer en un bucle infinito si la simulación se queda atrás, limitamos a un máximo de 5 pasos por frame.
        {
            EjecutarPaso();
            timerAcumulado -= updateTime;
            pasos++; // Incrementa el contador de pasos ejecutados en este frame para evitar ejecutar demasiados pasos si la simulación se queda atrás.
        }
    }

    private void EjecutarPaso() // Ejecuta un paso de simulación, incrementando el tiempo de simulación y el contador de pasos, y disparando el evento OnPaso para que otros componentes puedan actualizar su estado según el nuevo paso.
    {
        TiempoSimulacion += updateTime; // Incrementa el tiempo total de simulación por el tamaño del paso definido en updateTime, lo que representa el avance del tiempo en la simulación.
        ContadorPasos++; // Incrementa el contador de pasos de simulación ejecutados
        OnPaso?.Invoke(updateTime); // Dispara el evento OnPaso, pasando el tamaño del paso (Δt) para que los suscriptores puedan actualizar su estado según el nuevo paso de simulación.
    }

    public void Play() => estaPausada = false; // Método para reanudar la simulación si está pausada, simplemente estableciendo estaPausada en false.

    public void Pausar() => estaPausada = true; // Método para pausar la simulación, estableciendo estaPausada en true, lo que hará que el Update deje de acumular tiempo y ejecutar pasos hasta que se llame a Play() o AlternarPausa().

    public void AlternarPausa() => estaPausada = !estaPausada; // Método para alternar el estado de pausa de la simulación, cambiando estaPausada al valor opuesto.

    public void Resetear() // Método para resetear la simulación, reiniciando el tiempo acumulado, el tiempo de simulación y el contador de pasos a cero, y disparando el evento OnReset para que otros componentes puedan reiniciar su estado si es necesario.
    {
        timerAcumulado = 0f;
        TiempoSimulacion = 0f;
        ContadorPasos = 0;
        OnReset?.Invoke();
    }
}
