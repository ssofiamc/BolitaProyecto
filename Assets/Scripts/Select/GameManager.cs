using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string pokemonSeleccionado;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}