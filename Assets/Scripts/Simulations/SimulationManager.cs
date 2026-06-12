using UnityEngine;
using System.Collections.Generic;

public class SimulationManager: MonoBehaviour
{
    private static readonly List<Bolita> bolitas = new(); // Lista de todas las bolitas registradas en el simulador
    private static readonly List<IForceGenerator> generadores = new(); // Lista de todos los generadores de fuerza registrados en el simulador

    public static void Registrar(Bolita p)
    {
        if (!bolitas.Contains(p)) bolitas.Add(p); // Evita agregar la misma bolita más de una vez
    }
    public static void Desregistrar(Bolita p)
    {
        bolitas.Remove(p); // Elimina la bolita de la lista cuando ya no es necesaria (por ejemplo, al destruirla)
    }
    public static void Registrar(IForceGenerator g)
    {
        if (!generadores.Contains(g)) generadores.Add(g); // Evita agregar el mismo generador de fuerza más de una vez
    }
    public static void Desregistrar(IForceGenerator g)
    {
        generadores.Remove(g); // Elimina el generador de fuerza de la lista cuando ya no es necesario (por ejemplo, al destruirlo)
    }

    public static IReadOnlyList<Bolita> Todas => bolitas; // Propiedad de solo lectura para acceder a la lista de bolitas desde otros componentes sin permitir modificaciones directas a la lista.

    private void Start() // Suscribir eventos de BolitaSimulator
    {
        Debug.Log("SimulationManager Start");

        if (BolitaSimulator.Instancia == null)
        {
            Debug.LogError("No existe BolitaSimulator en la escena.");
            return;
        }

        Debug.Log("SimulationManager Suscrito");

        BolitaSimulator.Instancia.OnPaso += Paso;
        BolitaSimulator.Instancia.OnReset += ResetearTodo;
    } // Suscribirse a los eventos de paso y reset del BolitaSimulator para ejecutar los métodos correspondientes en cada paso de simulación y al resetear.

    private void OnDestroy() // Desuscribir eventos de BolitaSimulator para evitar referencias a objetos destruidos y posibles errores al intentar acceder a ellos después de que este objeto haya sido destruido.
    {
        if (BolitaSimulator.Instancia == null)
            return;

        BolitaSimulator.Instancia.OnPaso -= Paso;
        BolitaSimulator.Instancia.OnReset -= ResetearTodo;
    } // Desuscribirse de los eventos de BolitaSimulator para evitar referencias a objetos destruidos y posibles errores al intentar acceder a ellos después de que este objeto haya sido destruido.

    private void Paso(float dt) // Método que se ejecuta en cada paso de simulación, donde se aplican las fuerzas, se integran las posiciones y velocidades, se manejan las colisiones y se actualizan las visuales de las bolitas.
    {
        // 1. Todos los generadores de fuerza aplican sus fuerzas
        foreach (IForceGenerator g in generadores)
            g.ApplyForces(dt);

        // 2. Integración: acumula fuerzas → velocidad
        foreach (Bolita p in bolitas)
            p.Integrar(dt);

        // 3. Movimiento + colisiones contra paredes
        foreach (Bolita p in bolitas)
            p.Mover(dt);

        // 4. Sincronizar Transform con posición física
        foreach (Bolita p in bolitas)
            p.ActualizarVisuales();
    }

    private void ResetearTodo() // Método que se ejecuta al resetear la simulación, donde se llama al método Resetear() de cada bolita para reiniciar su estado a las condiciones iniciales definidas en su Awake() o Start().
    {
        foreach (Bolita p in bolitas)
            p.Resetear(); // Llama al método Resetear() de cada bolita para reiniciar su estado a las condiciones iniciales definidas en su Awake() o Start().
    }
}
