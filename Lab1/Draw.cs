using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    // the texture we will be drawing on
    Texture2D texture;
    //

    Vector2? p1, p2;

    void Start()
    {
        // init the 2 points
        p1 = null;
        p2 = null;
        // get the pad object and set it's texture to our texture
        GameObject pad = GameObject.Find("Pad");
        texture = new Texture2D(512, 512);
        pad.GetComponent<Renderer>().material.mainTexture = texture;
        // set it to all blank (white)
        SetFillColor(color: Color.white);
    }

    private void SetFillColor(Color color)
    {
        // Helper function to fill texture with a color
        // get pixels
        Color[] fill_array = texture.GetPixels();
        // set all pixels to color
        for (int i = 0; i < fill_array.Length; i++)

            fill_array[i] = color;

        texture.SetPixels(fill_array);
        // apply so it is visible
        texture.Apply();
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
        // only draw/erase if only 1 of draw or erase are pressed
        if ((A | X) & !(B | Y))
        {
            // drawing color to red
            color = Color.red;
        } else if (!(A | X) & (B | Y))
        {
            // "erase" by setting back to defaut white color
            color = Color.white;
        } else
        {
            // user released the buttons or pressed an invalid button combo
            // stop drawing
            p1 = null;
            p2 = null;
            return;
        }
        // Debug.Log(string.Format("{0}{1}{2}{3}", A, B, X, Y));

        RaycastHit hit;
        // calculate direction of the pen
        Vector3 direction = transform.localRotation * transform.TransformDirection(Vector3.down);

        // check if ray in direction of pen hits the pad
        if (Physics.Raycast(transform.position, direction, out hit, transform.lossyScale.y/1.9f))
        {
            // Debug.Log(string.Format("hit coord: {0},{1}", hit.textureCoord.x, hit.textureCoord.y));
            // Debug.Log(string.Format("rotation quat: {0}, {1}, {2}", direction.x, direction.y, direction.z));

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;
            // save last 2 pixels where we are drawing and draw a line between them
            p2 = p1;
            p1 = pixelUV;
            if (p1!= null && p2 != null)
            {
                DrawLine((Vector2)p1, (Vector2)p2, color);
                texture.Apply();
            }
        } else
        {
            // exited the pad area, stop drawing
            p1 = null;
            p2 = null;
        }
    }

    private void DrawLine(Vector2 p1, Vector2 p2, Color col)
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
            texture.SetPixel((int)current.x, (int)current.y, col);
        }
    }
}
