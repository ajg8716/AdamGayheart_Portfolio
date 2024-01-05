using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //text box to draw to
    public Text uiText;

    //reference to collision manager
    [SerializeField]
    CollisionManager collisionManager;

    //strings for UI elements headers
    private string health = "Health: ";
    private string score = "\nScore: ";
    private string wave = "\nWave: ";

    // Update is called once per frame
    void Update()
    {
        //writes text of the UI elements
        uiText.text = health + collisionManager.player.health.ToString() + score + collisionManager.player.Score.ToString();
    }
}
