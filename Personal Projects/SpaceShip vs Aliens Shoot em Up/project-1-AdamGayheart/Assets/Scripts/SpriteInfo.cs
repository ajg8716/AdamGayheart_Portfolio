using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteInfo : MonoBehaviour
{
    //rectangle size
    [SerializeField]
    Vector2 rectSize = Vector2.one;

    //bool for colliding
    [SerializeField]
    bool isColliding = false;

    bool shielded = false;

    [SerializeField]
    //entity health
    public int health = 3; 

    //int for the score
    private int score = 0;

    //int for the wave player is on
    private int wave = 1;

    //float for a hitdelay
    float hitdelay = 0.5f;

    //properties
    //can make methods to return these aswell get it working
    public Vector2 RectMin
    {
        get
        {
            return new Vector3(transform.position.x - rectSize.x / 2f, transform.position.y - rectSize.y / 2f, 1);
        }
    }

    public Vector2 RectMax
    {
        get
        {
            return new Vector3(transform.position.x + rectSize.x / 2f, transform.position.y + rectSize.y / 2f, 1);
        }
    }
    public bool IsColliding
    {
        get { return isColliding; }
        set { isColliding = value; }
    }
    public bool Shielded
    {
        get { return shielded; }
        set { shielded = value; }
    }
    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public int Wave
    {
        get { return wave; }
        set { wave = value; }
    }

    //spriterender to edit
    SpriteRenderer spriteRenderer;

    //list of the sprites for the asset
    [SerializeField]
    public List<Sprite> sprites = new List<Sprite>();


    //bool to determine to switch the sprite
    bool switchSprite = false;

    //property for switchSprite
    public bool SwitchSprite
    {
        get { return switchSprite; }
        set { switchSprite = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //gets the sprite for the sprite renderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        //if the sprite list is greater than 0
        if(sprites.Count > 0)
        {
            //render first sprite in the list
            spriteRenderer.sprite = sprites[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if the sprite is colliding
        if (isColliding)
        {
            //show as red
            spriteRenderer.color = Color.red;

            hitdelay -= Time.deltaTime;

            //if the delay is over
            if (hitdelay <= 0 )
            {
                isColliding = false;
                hitdelay = 0.5f;
            }
        }
        //if the sprite is shielded (only for player)
        if (shielded)
        {
            spriteRenderer.color = Color.cyan;
        }
        //if sprite is not colliding or shielded
        else if(isColliding == false || Shielded == false)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
