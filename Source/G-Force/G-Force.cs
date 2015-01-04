using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace G_Force
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class G_Force : MonoBehaviour
    {
        double currentG;
        Color colorOut;
        float alpha;
        Texture2D whiteTexture = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        protected void Start()
        {
            //DontDestroyOnLoad(this);
            string path = KSPUtil.ApplicationRootPath.Replace(@"\", "/") + "/GameData/G-Force/White32x32.png";
            byte[] texture = File.ReadAllBytes(path);
            whiteTexture.LoadImage(texture);
            RenderingManager.AddToPostDrawQueue(3, new Callback(postDraw));
        }

        void postDraw()
        {
            //ScreenMessages.PostScreenMessage(currentG.ToString() + " / " + alpha.ToString());
            colorOut.a = alpha;
            GUI.color = colorOut;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteTexture);
            GUI.color = Color.white;
        }

        public void Update()
        {
            currentG = FlightGlobals.ActiveVessel.geeForce;

            if (currentG > 0)
            {
                colorOut = Color.black;
                alpha = (float)currentG / 9;
                if (currentG <= 1)
                {
                    alpha = 0;
                }
            } else {
                colorOut = Color.red;
                alpha = (float)Math.Abs(currentG) / 9;
            }
        }
    }
}
