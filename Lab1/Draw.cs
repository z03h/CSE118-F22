using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    // the texture we will be drawing on
    Texture2D texture, backTexture;
    // 

    Vector2? p1, p2;

    void Start()
    {
        // init the 2 points
        p1 = null;
        p2 = null;
        // get the pad object and set it's texture to our texture
        GameObject pad = GameObject.Find("Pad");
        GameObject back = GameObject.Find("BackPad");
        texture = new Texture2D(512, 512);
        backTexture = new Texture2D(512, 512);
        pad.GetComponent<Renderer>().material.mainTexture = texture;
        back.GetComponent<Renderer>().material.mainTexture = backTexture;
        // set it to all blank (white)
        SetFillColor(texture, color: Color.white);
        SetFillColor(backTexture, color: Color.white);
    }

    private void SetFillColor(Texture2D tex, Color color)
    {
        // Helper function to fill texture with a color
        // get pixels
        Color[] fill_array = tex.GetPixels();
        // set all pixels to color
        for (int i = 0; i < fill_array.Length; i++)
        
            fill_array[i] = color;
        
        tex.SetPixels(fill_array);
        // apply so it is visible
        tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        bool A, B, X, Y;
        Color color;
        // check if buttons are pressed
        A = OVRInput.Get(OVRInput.Button.One);
        B = OVRInput.Get(OVRInput.Button.Two);
        X = OVRInput.Get(OVRInput.Button.Three);
        Y = OVRInput.Get(OVRInput.Button.Four);

        bool shouldDraw = (A | X);
        bool shouldErase = (B | Y);

        // only draw/erase if only 1 of draw or erase are pressed
        if (shouldDraw & !shouldErase)
        {
            // drawing  color to red
            color = Color.red;
        } else if (shouldErase & !shouldDraw)
        {
            // "erase" by setting back to defaut white color
            color = Color.white;
        } else
        {
            // user released the buttons or pressed both buttons
            // stop drawing
            p1 = null;
            p2 = null;
            return;
        }
        // Debug.Log(string.Format("{0}{1}{2}{3}", A, B, X, Y));

        RaycastHit hit;
        // calculate direction of the pen
        Vector3 direction = transform.localRotation * transform.TransformDirection(Vector3.down);

        // check if ray in direction of pen hits a pad
        if (Physics.Raycast(transform.position, direction, out hit, transform.lossyScale.y/1.9f))
        {
            // Debug.Log(string.Format("hit coord: {0},{1}", hit.textureCoord.x, hit.textureCoord.y));
            // Debug.Log(string.Format("rotation quat: {0}, {1}, {2}", direction.x, direction.y, direction.z));

            if (hit.collider == null)
            {
                return;
            }
            GameObject hitPad = hit.collider.gameObject;
            Texture2D tex;
            if (hitPad.name == "Pad")
            {
                tex = texture;
            } else if (hitPad.name == "BackPad")
            {
                tex = backTexture;
            } else
            {
                return;
            }

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;
            // save last 2 pixels where we are drawing and draw a line between them
            p2 = p1;
            p1 = pixelUV;
            if (p1 != null && p2 != null)
            {
                if (shouldDraw)
                {
                    DrawLine(tex, (Vector2)p1, (Vector2)p2, color);
                }
                else if (shouldErase)
                {
                    EraseConnected(tex, (Vector2)p1, color);
                } else
                {
                    return;
                }
                tex.Apply();
            }
        } else
        {
            // exited the pad, don't draw anymore
            p1 = null;
            p2 = null;
        }
    }

    private void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        // Helper function to draw a line between 2 points
        Vector2 current = p1;
        // get distance between 2 points
        float step = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ratio = 0;

        while ((int)current.x != (int)p2.x || (int)current.y != (int)p2.y)
        {
            // use lerp to [distance] pixels betwen the 2 points at specific ratios
            current = Vector2.Lerp(p1, p2, ratio);
            // add step for next point
            ratio += step;
            // set the color at point
            tex.SetPixel((int)current.x, (int)current.y, col);
            tex.SetPixel((int)current.x + 1, (int)current.y, col);
            tex.SetPixel((int)current.x - 1, (int)current.y, col);
            tex.SetPixel((int)current.x, (int)current.y + 1, col);
            tex.SetPixel((int)current.x, (int)current.y - 1, col);

        }
    }

    private void EraseConnected(Texture2D tex, Vector2 p1, Color col)
    {
        if (p1.x < 0 | p1.x >= tex.width | p1.y < 0 | p1.y >= tex.height)
        {
            return;
        }
        Color current = tex.GetPixel((int)p1.x, (int)p1.y);
        if (current == Color.white)
        {
            return;
        }
        tex.SetPixel((int)p1.x, (int)p1.y, col);
        EraseConnected(tex, new Vector2(p1.x + 1, p1.y), col);
        EraseConnected(tex, new Vector2(p1.x - 1, p1.y), col);
        EraseConnected(tex, new Vector2(p1.x, p1.y + 1), col);
        EraseConnected(tex, new Vector2(p1.x, p1.y - 1), col);

    }
}
