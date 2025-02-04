using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> Frames
    {
        get { return frames; }
    }
    private List<Sprite> frames;
    private float frameRate;
    
    int currentFrame;
    private float timer;

    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate=0.16f)
    {
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[currentFrame];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count; // to make it into a loop when the index is higher than count
            spriteRenderer.sprite = frames[currentFrame];
            timer-= frameRate;
        }
    }

}
