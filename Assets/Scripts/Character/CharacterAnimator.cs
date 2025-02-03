using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> walkDownSprites;
    [SerializeField] private List<Sprite> walkUpSprites;
    [SerializeField] private List<Sprite> walkRightSprites;
    [SerializeField] private List<Sprite> walkLeftSprites;
    
    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    // States
    private SpriteAnimator walkDownAnim;
    private SpriteAnimator walkUpAnim;
    private SpriteAnimator walkRightAnim;
    private SpriteAnimator walkLeftAnim;
    
    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;
    
    // References
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        walkDownAnim = new SpriteAnimator(spriteRenderer, walkDownSprites);
        walkUpAnim = new SpriteAnimator(spriteRenderer, walkUpSprites);
        walkRightAnim = new SpriteAnimator(spriteRenderer, walkRightSprites);
        walkLeftAnim = new SpriteAnimator(spriteRenderer, walkLeftSprites);
        
        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var previousAnim = currentAnim;
        currentAnim = MoveX switch
        {
            1 => walkRightAnim,
            -1 => walkLeftAnim,
            _ => MoveY switch
            {
                1 => walkUpAnim,
                -1 => walkDownAnim,
                _ => currentAnim
            }
        };

        if(currentAnim != previousAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();
        
        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPreviouslyMoving = IsMoving;
    }
}
