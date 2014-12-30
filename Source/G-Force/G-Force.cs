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
        float initialSharpness;

        protected void Start()
        {
            DontDestroyOnLoad(this);
            initialSharpness = FlightCamera.fetch.sharpness;
            Debug.Log("Initial Camera Sharpness: " + initialSharpness.ToString());
        }

        public void Update()
        {
            currentG = FlightGlobals.ActiveVessel.geeForce;
            if (currentG >= 2f)
            {
                //UnityEngine.Camera.current.
            }
        }
    }
}
