using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace G_Force
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class G_Force : MonoBehaviour
    {
        double currentG;
        float initialFOV;
        Texture2D redout = new Texture2D(Screen.width, Screen.height);
        Texture2D blackout = new Texture2D(Screen.width, Screen.height);

        protected void Start()
        {
            DontDestroyOnLoad(this);
            initialFOV = FlightCamera.fetch.fovDefault;
            RenderingManager.AddToPostDrawQueue(3, new Callback(drawEffect));
        }

        private void drawEffect()
        {
            float factor = (float)currentG / 1.5f;
            ScreenMessages.PostScreenMessage(currentG.ToString() + " / " + factor.ToString());
            guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            

            if (factor > 1f)
            {
                //FlightCamera.fetch.SetFoV(initialFOV / factor);
                guiTexture.color = new Color(0.0f, 0.0f, 0.0f, factor / 10);
                guiTexture.enabled = true;
            }
            else
            {
                //FlightCamera.fetch.SetFoV(initialFOV);
                guiTexture.color = Color.clear;
                guiTexture.enabled = false;
            }
            
        }

        public void Update()
        {
            currentG = FlightGlobals.ActiveVessel.geeForce;
        }
    }
}
