using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonReset : MonoBehaviour
{
    public void Reiniciar(int Nescena)
    {
        SceneManager.LoadScene(Nescena);
    }
}
