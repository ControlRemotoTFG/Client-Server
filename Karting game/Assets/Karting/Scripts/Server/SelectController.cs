using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectController : MonoBehaviour
{
    [SerializeField]
    private Canvas SelectionMode;
    [SerializeField]
    private Server server;
    [SerializeField]
    private KartGame.KartSystems.KeyboardInput keyInput;
    [SerializeField]
    private KartGame.KartSystems.GamepadInput gamePadInput;
    [SerializeField]
    private KartGame.KartSystems.MobileInput mobileInput;
    [SerializeField]
    private KartGame.KartSystems.KartMovement kartMovement;
    [SerializeField]
    private GameObject directorTrigger;

    private void Start()
    {
        directorTrigger.SetActive(false);//CongelarJuego
        kartMovement.enabled = false;
    }

    public void ControlerSelected()
    {
        gamePadInput.enabled = true;
        keyInput.enabled = true;
        SelectionMode.enabled = false;
        mobileInput.enabled = false;
        kartMovement.input = keyInput;
        kartMovement.enabled = true;
        directorTrigger.SetActive(true);//DescongelarJuego
    }

    public void MobileSelected()
    { 
        mobileInput.enabled = true;
        SelectionMode.enabled = false;
        gamePadInput.enabled = false;
        keyInput.enabled = false;
        kartMovement.input = mobileInput;
        kartMovement.enabled = true;
        server.IniciarServer();
        server.AddListener(mobileInput);
        directorTrigger.SetActive(true);//DescongelarJuego
    }
}
