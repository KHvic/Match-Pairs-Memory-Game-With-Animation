﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _Card : MonoBehaviour
{

    private int spriteID;
    private int id;
    private bool flipped;
    private bool turning;
    [SerializeField]
    private Image img;

    // flip card animation
    // if changeSprite specified, will 90 degree, change to back/front sprite before flipping another 90 degree
    private IEnumerator Flip90(Transform thisTransform, float time, bool changeSprite)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }
        //change sprite and flip another 90degree
        if (changeSprite)
        {
            flipped = !flipped;
            ChangeSprite();
            StartCoroutine(Flip90(transform, time, false));
        }
        else
            turning = false;
    }
    // perform a 180 degree flip
    public void Flip()
    {
        turning = true;
        AudioPlayer.Instance.PlayAudio(0);
        StartCoroutine(Flip90(transform, 0.25f, true));
    }
    // toggle front/back sprite
    private void ChangeSprite()
    {
        if (spriteID == -1 || img == null) return;
        if (flipped)
            img.sprite = _CardGameManager.Instance.GetSprite(spriteID);
        else
            img.sprite = _CardGameManager.Instance.CardBack();
    }
    // call fade animation
    public void Inactive()
    {
        StartCoroutine(Fade());
    }
    // play fade animation by changing alpha of img's color
    private IEnumerator Fade()
    {
        float rate = 1.0f / 2.5f;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            img.color = Color.Lerp(img.color, Color.clear, t);

            yield return null;
        }
    }
    // set card to be active color
    public void Active()
    {
        if (img)
            img.color = Color.white;
    }
    // spriteID getter and setter
    public int SpriteID
    {
        set
        {
            spriteID = value;
            flipped = true;
            ChangeSprite();
        }
        get { return spriteID; }
    }
    // card ID getter and setter
    public int ID
    {
        set { id = value; }
        get { return id; }
    }
    // reset card default rotation
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        flipped = true;
    }
    // card onclick event
    public void CardBtn()
    {
        if (flipped || turning) return;
        if (!_CardGameManager.Instance.canClick()) return;
        Flip();
        StartCoroutine(SelectionEvent());
    }
    // inform manager card is selected with a slight delay
    private IEnumerator SelectionEvent()
    {
        yield return new WaitForSeconds(0.5f);
        _CardGameManager.Instance.cardClicked(spriteID, id);
    }
}
