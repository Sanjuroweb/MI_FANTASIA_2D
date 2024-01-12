using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour //30
{
    public static GameManager instance;
    public GameObject vidasUI;
    public PlayerController player; //37
    public TextMeshProUGUI textoMonedas; //37 HECHO POR MI, cambie el tipo de Text a TextMeshProUGUI porque en el inspector no me dejaba asignar al GameManager
    public int monedas;
    public Text guardarPartidaTexto; //46

    public GameObject panelPausa; //42
    public GameObject panelGameOver; //42
    public GameObject panelCarga; //42


    //hacemos un patron Singleton
    //Awake es un método especial en Unity que se llama cuando se inicializa un objeto antes de que se ejecute el método Start.
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        //46 recordar mirar el video y como borrar los datos del jugador desde unity
        //edit/clear all PlayerPrefs
        if(PlayerPrefs.GetInt("vidas") != 0)
            CargarPartida();
    }

    //46
    public void GuardarPartida()
    {
        float x, y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int vidas = player.vidas;

        //en el video se comenta que esta sacado de la web de unity
        //se usa para guardar los datos del player en memoria
        PlayerPrefs.SetInt("monedas", monedas);
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetInt("vidas", vidas);
    }

    //46 cargamos los datos guardados en PlayerPrefs
    //tenemos que llamarla desde el Awake()
    public void CargarPartida()
    {
        monedas = PlayerPrefs.GetInt("monedas");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.vidas = PlayerPrefs.GetInt("vidas");
        textoMonedas.text = monedas.ToString();

        int vidasADescontar = 3 - player.vidas; //46

        player.ActualizarVidasUI(vidasADescontar); //46
    }

    //37
    public void ActualizarContadorMonedas()
    {
        monedas++;
        textoMonedas.text = monedas.ToString();
    }

    //42
    public void PausarJuego()
    {
        //para que el juego quede pausado
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }
    
    //42
    public void DesPausarJuego()
    {
        //para que el juego continue
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    //42
    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    
    //42
    public void CargarSelector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    } 
    
    //42
    public void CargarEscena(string escenaACargar)
    {
        SceneManager.LoadScene(escenaACargar);
    } 
    
    //42
    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }
    
    //42
    public void SalirDelJuego()
    {
        //para salir del juego
        Application.Quit();
    }

    //42
    public void CargarEscenaSelector()
    {
        StartCoroutine(CargarEscena());
    }

    //42
    private IEnumerator CargarEscena()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncload = SceneManager.LoadSceneAsync("LevelSelect");

        //wait until the asynchronous scene fully loads
        while (asyncload.isDone)
        {
            //cuando tengamos una escena mas grande durará más tiempo la pantalla de carga
            yield return null;
            //para hacer que la pantalla de carga dure mas tiempo artificialmente
            //yield return new WaitForSeconds(1);
        }
    }
}
