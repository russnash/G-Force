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
        Texture2D whiteTexture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        Color colorOut = new Color();

        /// <summary>
        /// This is the cumulative effect of G's which is increasingly negative to represent a redout condition 
        /// and increasingly positive to represent a blackout condition. As G forces continue, the cumulativeG
        /// value is increased, by larger amounts for higher current G levels, to allow for longer periods of G
        /// force tolerance at lower G levels.
        /// </summary>
        double cumulativeG = 0;
        double currentG;
        double factor;
        bool isPositiveG = true;
        CelestialBody mainBody;
        Vector3d mainBodyCOM;

        protected void Start()
        {
            // Load the 32x32 pure white png texture
            string path = KSPUtil.ApplicationRootPath.Replace(@"\", "/") + "/GameData/G-Force/White32x32.png";
            byte[] texture = File.ReadAllBytes(path);
            whiteTexture.LoadImage(texture);

            // Hook into the rendering queue to draw the G effects
            RenderingManager.AddToPostDrawQueue(3, new Callback(drawGEffects));

            // Add another rendering queue hook for the GUI
            RenderingManager.AddToPostDrawQueue(4, new Callback(drawGUI));
        }

        void drawGEffects()
        {
            if (isPositiveG)
            {
                colorOut = Color.black;
                colorOut.a = (float)cumulativeG / 32767;
            }
            else
            {
                colorOut = Color.red;
                colorOut.a = (float)currentG / 2;
            }

            //ScreenMessages.PostScreenMessage(cumulativeG.ToString() + " / " + colorOut.a.ToString());

            GUI.color = colorOut;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteTexture);
            GUI.color = Color.white;
        }

        void drawGUI()
        {

        }

        public void Update()
        {
            // Retrieve the orbited celestial body.
            mainBody = FlightGlobals.ActiveVessel.mainBody;

            // Retrieve the position of the main orbited bodies COM in world coords.
            mainBodyCOM = mainBody.position;

            // Retrieve the current G forces.
            currentG = FlightGlobals.ActiveVessel.geeForce;

            // If we're experiencing 4G's plus, grow the cumulativeG with relation to the current G and ensure
            // we don't wrap to a negative value.
            if (currentG >= 4)
            {
                factor = currentG * currentG;
                if (32767 - cumulativeG <= factor)
                {
                    cumulativeG = 32767;
                }
                else
                {
                    cumulativeG += factor;
                }
            }
            // If we're experiencing between 0 and 4G's, reduce the cumulativeG with relation to the current G
            // and ensure we don't drop below 0.
            else
            {
                factor = 10 - currentG;
                cumulativeG -= factor * factor;
                if (cumulativeG < 0)
                {
                    cumulativeG = 0;
                }
            }
        }
    }
}
