using Horror.Player;
using System;
using TMPro;
using UnityEngine;

public class InteractiveScript : MonoBehaviour
{
    [SerializeField] private string nameObj;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    public string NameObj { get => nameObj; set => nameObj = value; }

    public void OpenDoor()
    {
        Debug.Log("Open Door");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().OBJ = this;
            SetText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ClearText();
            other.GetComponent<Player>().OBJ = null;
        }
    }

    private void SetText()
    {
        if (nameObj == "key")
        {
            textMeshProUGUI.text = ("Você encontrou uma chave, aperte 'E' para pegá-la");
        }
        if (nameObj == "battery")
        {
            textMeshProUGUI.text = ("Você encontrou baterias, aperte 'E' para pegá-las");
        }
        if (nameObj == "door")
        {
            textMeshProUGUI.text = ("Você precisa de uma chave para abrir esta porta, aperte 'E' para abrir");
        }
    }

    private void ClearText()
    {
        textMeshProUGUI.text = ("");
    }

    private void OnDestroy()
    {
        ClearText();
    }
}
