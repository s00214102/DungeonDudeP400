using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//-copied from youtube video https://www.youtube.com/watch?v=iD1_JczQcFY
//-modified by Marc McCabe to be a little simplier

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private void Awake()
    {
        textMesh = transform.GetComponentInChildren<TextMeshPro>();
    }
    private void Update()
    {
        float moveYSpeed = 1.1f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            //start dissappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTimer = 0.6f;
    }
    public void Setup(int damageAmount, Color m_textColor)
    {
        //override for setting color
        textMesh.SetText(damageAmount.ToString());
        textMesh.color = m_textColor;
        textColor = textMesh.color;
        disappearTimer = 0.6f;
    }
    public void Setup(int damageAmount, Color m_textColor, float m_textSize)
    {
        //override for setting color and size
        textMesh.SetText(damageAmount.ToString());
        textMesh.color = m_textColor;
        textMesh.fontSize = m_textSize;
        textColor = textMesh.color;
        disappearTimer = 0.6f;
    }
}
