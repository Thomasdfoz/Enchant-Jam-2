using Horror.Player;
using Michsky.UI.Dark;
using UnityEngine;

public class MenuGame : MonoBehaviour
{
    [SerializeField] BlurManager BlurManager;
    [SerializeField] ModalWindowManager ModalWindowManager;
    [SerializeField] Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            BlurManager.BlurInAnim();
            ModalWindowManager.ModalWindowIn();
            player.IsPaused = true;
        }
    }

    public void Play()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.IsPaused = false;
    }




}
