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
        double distOrbCOM;
        double distOrbCOM2AccellPos;

        protected void Start()
        {
            // Load the 32x32 pure white png texture
            string path = KSPUtil.ApplicationRootPath.Replace(@"\", "/") + "/GameData/G-Force/White32x32.png";
            byte[] texture = File.ReadAllBytes(path);
            whiteTexture.LoadImage(texture);

            // Hook into the rendering queue to draw the G effects
            RenderingManager.AddToPostDrawQueue(3, new Callback(drawGEffects));

            // Add another rendering queue hook for the GUI
            //RenderingManager.AddToPostDrawQueue(4, new Callback(drawGUI));
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
                colorOut.a = (float)currentG / 10;
            }

            //ScreenMessages.PostScreenMessage(cumulativeG.ToString());

            if (FlightGlobals.ActiveVessel.horizontalSrfSpeed > 50 & FlightGlobals.ActiveVessel.situation != Vessel.Situations.LANDED) // Dirty hack to stop the 'drunk prograde marker' causing weird G effects when stopped
            {
                GUI.color = colorOut;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), whiteTexture);
                GUI.color = Color.white;
            }
        }

        void drawGUI()
        {
            // Good in 'ere, init!
        }

        public void Update()
        {
            // Retrieve the center of mass of the orbited celestial body.
            mainBody = FlightGlobals.ActiveVessel.mainBody;
            mainBodyCOM = mainBody.position;

            // Retrieve the current G forces.
            currentG = FlightGlobals.ActiveVessel.geeForce;

            // Calculate the distance from the orbited bodies COM to the current vessels COM
            distOrbCOM = Vector3d.Distance(mainBodyCOM, FlightGlobals.ActiveVessel.GetWorldPos3D());

            // Calculate the distance from the orbited bodies COM to the current vessels COM with the accelleration vector applied
            distOrbCOM2AccellPos = Vector3d.Distance(mainBodyCOM, FlightGlobals.ActiveVessel.GetWorldPos3D() + FlightGlobals.ActiveVessel.acceleration);

            // If the vessels position plus its acceleration vector is further from the orbited bodies COM than the vessel itself is,
            // then we are experiencing positive G's, otherwise negative.
            if (distOrbCOM2AccellPos > distOrbCOM)
            {
                isPositiveG = true;
            }
            else
            {
                isPositiveG = false;
            }

            // Now, the above calculations assume that we are right-side up (not flying upside down!), so, if we are inverted we need
            // to invert the G type.


            // If we're experiencing 4G's plus, grow the cumulativeG with relation to the current G and ensure
            // we don't wrap to a negative value.
            if (currentG >= 4)
            {
                factor = currentG * (currentG / 4);
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
