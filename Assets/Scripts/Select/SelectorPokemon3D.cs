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
        for (int i = 0; i < pokemones.Length; i++)
        {
            pokemones[i].SetActive(i == indiceActual);
        }

        nombrePokemon.text = pokemones[indiceActual].name;
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