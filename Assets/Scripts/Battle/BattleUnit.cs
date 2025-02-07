using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHud hud;

    private Image image;
    private Color originalColor;
    private Vector3 originalPos;

    public bool IsPlayerUnit => isPlayerUnit;

    public BattleHud Hud => hud;

    public Pokemon Pokemon { get; set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;

        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);

        image.color = originalColor;

        PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-600f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(600f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();

        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.DOColor(Color.gray, .1f));
        seq.Append(image.DOColor(originalColor, .1f));
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, .5f));
        seq.Join(image.DOFade(0f, .5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.DOFade(0f, .5f));
        seq.Join(transform.DOLocalMoveY(originalPos.y + 150f, .5f));
        seq.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), .5f));
        yield return seq.WaitForCompletion();
    }
}