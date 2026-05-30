using UnityEngine;

public class ImpulseSimulation : MonoBehaviour
{
    [Header("Configuración del Impulso")]
    [Tooltip("Dirección del impulso (en espacio mundo). Se normalizará.")]
    public Vector3 direccionImpulso = Vector3.forward;

    [Tooltip("Magnitud del impulso J (kg·m/s). Se divide entre la masa → Δv.")]
    public float magnitudImpulso = 15f;

    [Header("Cooldown")]
    [Tooltip("Tiempo de espera entre activaciones (segundos).")]
    [Min(0f)] public float tiempoCooldown = 1.5f;

    [Header("Efectos Visuales")]
    [Tooltip("Partículas de activación (opcional, asignar un ParticleSystem).")]
    public ParticleSystem efectoActivacion;

    [Tooltip("Color del pad cuando está listo / en cooldown.")]
    public Color colorListo = Color.yellow;
    public Color colorCooldown = Color.gray;

    // ─────────────────────────────────────────────
    // ESTADO INTERNO
    // ─────────────────────────────────────────────
    private float timerCooldown = 0f;
    private bool enCooldown = false;
    private Renderer rend;

    // ─────────────────────────────────────────────
    // INICIO
    // ─────────────────────────────────────────────
    private void Start()
    {
        rend = GetComponent<Renderer>();
        ActualizarColor();
    }

    // ─────────────────────────────────────────────
    // UPDATE: gestión del cooldown
    // ─────────────────────────────────────────────
    private void Update()
    {
        if (enCooldown)
        {
            timerCooldown -= Time.deltaTime;
            if (timerCooldown <= 0f)
            {
                enCooldown = false;
                ActualizarColor();
            }
        }
    }

    // ─────────────────────────────────────────────
    // TRIGGER: cuando la bolita entra al pad
    // Se usa OnTriggerEnter (detección) + AddImpulso (físico manual)
    // ─────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (enCooldown) return;
        if (!other.TryGetComponent<Bolita>(out var bolita)) return;

        // Calcular el impulso: J = magnitud × dirección_normalizada
        Vector3 impulso = direccionImpulso.normalized * magnitudImpulso;

        // Aplicar: Δv = J / m  (internamente en AgregarImpulso)
        bolita.AgregarImpulso(impulso);

        // Log de energía para sustentación
        float ek = 0.5f * bolita.Masa * bolita.Velocidad.sqrMagnitude;
        Debug.Log($"[Impulso] Pad activado. J = {magnitudImpulso} kg·m/s. " +
                  $"Ek post-impulso ≈ {ek:F2} J");

        // Activar efectos
        if (efectoActivacion != null)
            efectoActivacion.Play();

        // Iniciar cooldown
        enCooldown = true;
        timerCooldown = tiempoCooldown;
        ActualizarColor();
    }

    // ─────────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────────
    private void ActualizarColor()
    {
        if (rend == null) return;
        rend.material.color = enCooldown ? colorCooldown : colorListo;
    }

    // ─────────────────────────────────────────────
    // GIZMOS
    // ─────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direccionImpulso.normalized * 2.5f);
        Gizmos.DrawWireSphere(transform.position + direccionImpulso.normalized * 2.5f, 0.2f);
    }
}
