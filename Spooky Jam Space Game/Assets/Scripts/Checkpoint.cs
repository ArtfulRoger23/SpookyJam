using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private Sprite activateSprite;
    [SerializeField]
    private bool activated;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public bool Activated { get => activated; }

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (activated)
            return;

        activated = true;
        spriteRenderer.sprite = activateSprite;
    }
}
