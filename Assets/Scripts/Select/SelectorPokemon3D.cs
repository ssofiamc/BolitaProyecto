using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectorPokemon3D : MonoBehaviour
{
    public GameObject[] pokemones;
    public TextMeshProUGUI nombrePokemon;

    private int indiceActual = 0;

    void Start()
    {
        MostrarPokemon();
    }

    void MostrarPokemon()
    {
        if (pokemones == null || pokemones.Length == 0)
        {
            Debug.LogError("No hay Pokémon asignados.");
            return;
        }

        Debug.Log("Mostrando: " + pokemones[indiceActual].name);

        for (int i = 0; i < pokemones.Length; i++)
        {
            Transform visual = pokemones[i].transform.Find("Visual");

            if (visual == null)
            {
                Debug.LogError("No encontré el hijo 'Visual' en: " + pokemones[i].name);
                continue;
            }

            bool activo = (i == indiceActual);

            visual.gameObject.SetActive(activo);

            Debug.Log(
                pokemones[i].name +
                " -> Visual activo = " +
                activo
            );
        }

        if (nombrePokemon != null)
        {
            nombrePokemon.text = pokemones[indiceActual].name;
        }
    }

    public void Siguiente()
    {
        indiceActual++;

        if (indiceActual >= pokemones.Length)
        {
            indiceActual = 0;
        }

        MostrarPokemon();
    }

    public void Anterior()
    {
        indiceActual--;

        if (indiceActual < 0)
        {
            indiceActual = pokemones.Length - 1;
        }

        MostrarPokemon();
    }

    public void Aceptar()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pokemonSeleccionado =
                pokemones[indiceActual].name;
        }

        SceneManager.LoadScene("GameScene");
    }
}