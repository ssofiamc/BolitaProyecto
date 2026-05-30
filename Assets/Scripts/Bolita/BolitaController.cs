using UnityEngine;

public class BolitaController : MonoBehaviour
{
    [Header("Fisica")]
    public float fuerzaMovimiento = 15f;
    public float velocidadMaxima = 8f;
    public float gravedad = -20f;
    public float drag = 4f;

    [Header("Arduino (opcional)")]
    public float normalizacionGiro = 80f;

    private Vector3 velocidad = Vector3.zero;
    private float radio;
    private Collider[] buffer = new Collider[8];

    void Start()
    {
        radio = transform.localScale.x * 0.5f;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // ── INPUT ──────────────────────────────────────────────
        float ix = 0f, iz = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) ix = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ix = 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) iz = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) iz = -1f;

        ix = Mathf.Clamp(ix + Mathf.Clamp(MPU6050Reader.x / normalizacionGiro, -1f, 1f), -1f, 1f);
        iz = Mathf.Clamp(iz + Mathf.Clamp(MPU6050Reader.y / normalizacionGiro, -1f, 1f), -1f, 1f);

        // ── ACELERACIÓN ────────────────────────────────────────
        velocidad.x += ix * fuerzaMovimiento * dt;
        velocidad.z += iz * fuerzaMovimiento * dt;
        velocidad.y += gravedad * dt;

        // Drag horizontal
        velocidad.x = Mathf.MoveTowards(velocidad.x, 0f, drag * dt);
        velocidad.z = Mathf.MoveTowards(velocidad.z, 0f, drag * dt);

        // Limitar velocidad horizontal
        Vector3 velH = new Vector3(velocidad.x, 0f, velocidad.z);
        if (velH.magnitude > velocidadMaxima)
        {
            velH = velH.normalized * velocidadMaxima;
            velocidad.x = velH.x;
            velocidad.z = velH.z;
        }

        // ── MOVER ──────────────────────────────────────────────
        transform.position += velocidad * dt;

        // ── RESOLVER PENETRACIONES (depenetration) ─────────────
        // Esto es lo que impide que atraviese el suelo y las paredes
        ResolverPenetraciones();
    }

    void ResolverPenetraciones()
    {
        // Busca todos los colliders que se solapan con la bolita ahora mismo
        int cantidad = Physics.OverlapSphereNonAlloc(
            transform.position, radio, buffer,
            ~0, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < cantidad; i++)
        {
            Collider col = buffer[i];

            // Ignorar el propio collider de la bolita
            if (col.gameObject == gameObject) continue;

            // Calcular dirección y distancia de penetración
            if (Physics.ComputePenetration(
                GetComponent<Collider>(), transform.position, transform.rotation,
                col, col.transform.position, col.transform.rotation,
                out Vector3 direccion, out float distancia))
            {
                // Empujar la bolita fuera del collider
                transform.position += direccion * (distancia + 0.001f);

                // Cancelar velocidad en esa dirección
                float vEnDir = Vector3.Dot(velocidad, -direccion);
                if (vEnDir < 0f)
                    velocidad += direccion * vEnDir;
            }
        }
    }
}
