using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransicion : MonoBehaviour //47
{
    //47
    public void AparecerJuego()
    {
        gameObject.GetComponent<Animator>().SetTrigger("aparecer");
    }

    //47
    public void DefaultTransicion()
    {
        gameObject.GetComponent<Animator>().SetTrigger("default");
    }
}
