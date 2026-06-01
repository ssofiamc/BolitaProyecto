using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectorPokemon3D : MonoBehaviour
{
    public GameObject[] pokemones;
    public Transform puntoSpawn;
    public TextMeshProUGUI nombrePokemon;

    private int indiceActual = 0;
    private GameObject pokemonActual;

    void Start()
    {
        MostrarPokemon();
    }

    void MostrarPokemon()
    {
        if (pokemonActual != null)
            Destroy(pokemonActual);

        pokemonActual = Instantiate(
            pokemones[indiceActual],
            puntoSpawn.position,
            Quaternion.identity
        );

        nombrePokemon.text =
            pokemones[indiceActual].name;
    }

    public void Siguiente()
    {
        indiceActual++;

        if (indiceActual >= pokemones.Length)
            indiceActual = 0;

        MostrarPokemon();
    }

    public void Anterior()
    {
        indiceActual--;

        if (indiceActual < 0)
            indiceActual = pokemones.Length - 1;

        MostrarPokemon();
    }

    public void Aceptar()
    {
        GameManager.Instance.pokemonSeleccionado =
            pokemones[indiceActual].name;

        SceneManager.LoadScene("GameScene");
    }
}