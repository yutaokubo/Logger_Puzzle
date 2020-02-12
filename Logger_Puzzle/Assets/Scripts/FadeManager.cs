using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;
    

    private enum FadeState
    {
        In,
        Out,
        End,
    }
    [SerializeField]
    private FadeState state;

    [SerializeField]
    private float fadeTime;
    private float fadeTimer;

    private float r, b, g;
    private float a;

    // Start is called before the first frame update
    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        r = fadeImage.color.r;
        b = fadeImage.color.b;
        g = fadeImage.color.g;
        a = fadeImage.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        FadeInUpdate();
        FadeOutUpdate();
    }

    public void FadeInStart()
    {
        a = 1;
        fadeTimer = 0;
        state = FadeState.In;
        fadeImage.color = new Color(r, g, b, a);
        Debug.Log("FadeIn");
    }

    private void FadeInUpdate()
    {
        if (state != FadeState.In)
            return;

        fadeTimer += Time.deltaTime;
        a -=  1/fadeTime * Time.deltaTime;
        if(a<=0)
        {
            state = FadeState.End;
            fadeTimer = 0;
            a = 0;
        }
        fadeImage.color = new Color(r, g, b, a);
    }

    public void FadeOutStart()
    {
        a = 0;
        fadeTimer = 0;
        state = FadeState.Out;
        fadeImage.color = new Color(r, g, b, a);
    }
    private void FadeOutUpdate()
    {
        if (state != FadeState.Out)
            return;

        fadeTimer += Time.deltaTime;
        a += 1 / fadeTime * Time.deltaTime;
        if (a >= 1)
        {
            state = FadeState.End;
            fadeTimer = 0;
            a = 1;
        }
        fadeImage.color = new Color(r, g, b, a);
    }

    public bool IsFadeEnd()
    {
        return state == FadeState.End;
    }
}
