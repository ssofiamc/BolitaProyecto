using UnityEngine;
using System.Collections.Generic;

public class SimulationManager: MonoBehaviour
{
    private static readonly List<Bolita> bolitas = new();
    private static readonly List<IForceGenerator> generadores = new();

    public static void Registrar(Bolita p)
    {
        if (!bolitas.Contains(p)) bolitas.Add(p);
    }
    public static void Desregistrar(Bolita p)
    {
        bolitas.Remove(p);
    }
    public static void Registrar(IForceGenerator g)
    {
        if (!generadores.Contains(g)) generadores.Add(g);
    }
    public static void Desregistrar(IForceGenerator g)
    {
        generadores.Remove(g);
    }

    public static IReadOnlyList<Bolita> Todas => bolitas;

    // ─────────────────────────────────────────────
    // SUSCRIPCIÓN AL SIMULADOR
    // ─────────────────────────────────────────────
    private void OnEnable()
    {
        if (BolitaSimulator.Instancia != null)
        {
            BolitaSimulator.Instancia.OnPaso += Paso;
            BolitaSimulator.Instancia.OnReset += ResetearTodo;
        }
    }

    private void OnDisable()
    {
        if (BolitaSimulator.Instancia != null)
        {
            BolitaSimulator.Instancia.OnPaso -= Paso;
            BolitaSimulator.Instancia.OnReset -= ResetearTodo;
        }
    }

    // ─────────────────────────────────────────────
    // CICLO DE SIMULACIÓN
    // ─────────────────────────────────────────────
    private void Paso(float dt)
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

    private void ResetearTodo()
    {
        foreach (Bolita p in bolitas)
            p.Resetear();
    }
}
