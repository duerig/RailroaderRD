using Railroader.Modding;
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Model.Ops;
using Railroader;
using Game.Scripting.Modules.Railroader;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Character;
using Model;
using Game.State;
using Game.Messages;
using Effects;
using Model.AI;
using System.Runtime.Remoting.Messaging;
using Game.Scripting.Modding;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using HarmonyLib;
using KeyValue.Runtime;
using UI;
using UI.Builder;

namespace RailroaderRD
{
    public sealed class EntryPoint : IModEntryPoint, IWorldLifecycle
    {
        Ticker ticker;
        IModContext context;
        RaildriverInterface raildriver;
        String savepath;
        public void OnModLoad(IModContext context_in)
        {
            context = context_in;
            savepath = context.ModDirectory + "\\caldata.xml";
            if (raildriver == null)
            {
                raildriver = InputSystem.AddDevice<RaildriverInterface>();
                raildriver.context = context;
            }
            raildriver.Connect();
            XmlSerializer serializer = new XmlSerializer(typeof(CalibrationData));
            FileStream fs = new FileStream(savepath, FileMode.Open);
            raildriver.CalibrationData = (CalibrationData) serializer.Deserialize(fs);
            HubLink.Register("duerig/rrrd", "Railroader Raildriver Support", "1.0", CalibrationMenu);
            HubFallback.Enable("duerig/rrrd", "Railroader Raildriver Support", CalibrationMenu, "<Keyboard>/f7", ctrl: true);
        }

        public void OnModUnload()
        {
            if (raildriver != null)
            {
                raildriver.Disconnect();
            }
            HubLink.Unregister();
        }

        public void OnWorldLoad()
        {
            context.Log.Info($"Loaded from {context.ModDirectory}");
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            if (roots.Length > 0)
            {
                // ticker = new Ticker(context);
                ticker = roots[0].AddComponent<Ticker>();
                ticker.context = context;
                ticker.input = raildriver;
            }
        }

        public void OnWorldUnload()
        {
        }

        private void SaveConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalibrationData));
            TextWriter writer = new StreamWriter(savepath);
            serializer.Serialize(writer, raildriver.CalibrationData);            
        }

        private void CalibrationMenu(UIPanelBuilder builder)
        {
            builder.ConfigureLayout(null, new UnityEngine.RectOffset(20, 20, 20, 20), null);
            builder.AddTitle("Calibration for Raildriver controls", null);
            builder.AddLabel("Move the specified control to each position and click the indicated button. Afterwards, verify that the slider moves smoothly along its length as you move the control.");
            builder.Spacer(10);
            builder.AddHRule();
            builder.Spacer(20);
            builder.AddLabel("Reverser (Left Lever) -- F = top, N = middle, R = bottom");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("F", () => { raildriver.CalibrationData.ReverserMin = raildriver.RawReverser; SaveConfig(); });
                hstack.AddButtonCompact("N", () => { raildriver.CalibrationData.ReverserCenter = raildriver.RawReverser; SaveConfig(); });
                hstack.AddButtonCompact("R", () => { raildriver.CalibrationData.ReverserMax = raildriver.RawReverser; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.Reverser, () => "", (x) => { }, -1.0f, 1.0f);
            builder.Spacer(20);

            builder.AddLabel("Dynamic Brake/Throttle (Left Middle Lever) -- Brk = top, 0 = resting just below the middle notch, Throttle = bottom");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("Brk", () => { raildriver.CalibrationData.ThrottleMin = raildriver.RawThrottle; SaveConfig(); });
                hstack.AddButtonCompact("0", () => { raildriver.CalibrationData.ThrottleCenter = raildriver.RawThrottle; SaveConfig(); });
                hstack.AddButtonCompact("Throttle", () => { raildriver.CalibrationData.ThrottleMax = raildriver.RawThrottle; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.Throttle, () => "", (x) => { }, -1.0f, 1.0f);
            builder.Spacer(20);

            builder.AddLabel("Auto Brake (Right Middle Lever) -- Emg = top, Full = resting just below the top notch, Release = bottom");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("Emg", () => { raildriver.CalibrationData.AutoBrakeMin = raildriver.RawAutoBrake; SaveConfig(); });
                hstack.AddButtonCompact("Full", () => { raildriver.CalibrationData.AutoBrakeEmg = raildriver.RawAutoBrake; SaveConfig(); });
                hstack.AddButtonCompact("Release", () => { raildriver.CalibrationData.AutoBrakeMax = raildriver.RawAutoBrake; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.AutoBrake, () => "", (x) => { }, 0.0f, 1.0f);
            builder.Spacer(20);

            builder.AddLabel("Ind Brake (Right Lever) -- Full = top, Release = bottom");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("Full", () => { raildriver.CalibrationData.IndBrakeMin = raildriver.RawIndBrake; SaveConfig(); });
                hstack.AddButtonCompact("Release", () => { raildriver.CalibrationData.IndBrakeMax = raildriver.RawIndBrake; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.IndBrake, () => "", (x) => { }, 0.0f, 1.0f);
            builder.Spacer(20);

            builder.AddLabel("Bail Off (Right Lever) -- 0 = left, 1 = right");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("0", () => { raildriver.CalibrationData.BailOffMin = raildriver.RawBailOff; SaveConfig(); });
                hstack.AddButtonCompact("1", () => { raildriver.CalibrationData.BailOffMax = raildriver.RawBailOff; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.BailOff, () => "", (x) => { }, 0.0f, 1.0f);
            builder.Spacer(20);

            builder.AddLabel("Enable (top-right rotary) -- Off = left, On = right");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("Off", () => { raildriver.CalibrationData.WiperMin = raildriver.RawWipers; SaveConfig(); });
                hstack.AddButtonCompact("On", () => { raildriver.CalibrationData.WiperMax = raildriver.RawWipers; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.Wipers, () => "", (x) => { }, 0.0f, 1.0f);
            builder.Spacer(20);

            builder.AddLabel("Lights (bottom-right rotary) -- Off = left, Dim = middle, Full = right");
            builder.HStack(delegate (UIPanelBuilder hstack) {
                hstack.AddButtonCompact("Off", () => { raildriver.CalibrationData.LightsMin = raildriver.RawLights; SaveConfig(); });
                hstack.AddButtonCompact("Dim", () => { raildriver.CalibrationData.LightsCenter = raildriver.RawLights; SaveConfig(); });
                hstack.AddButtonCompact("Full", () => { raildriver.CalibrationData.LightsMax = raildriver.RawLights; SaveConfig(); });
            });
            builder.AddSlider(() => raildriver.Lights, () => "", (x) => { }, 0.0f, 1.0f);
            builder.Spacer(20);

            builder.AddTitle("Other Controls", null);
            builder.AddLabel("Left tall switch for whistle");
            builder.AddLabel("Bottom-right button on left panel to toggle bell");
       }
    }

    public sealed class Ticker : MonoBehaviour
    {
        public IModContext context;
        public RaildriverInterface input;
        private ButtonMask prevMask = 0;
        private int arity = 0;
        private float indBrake = 0.0f;
        private float trainBrake = 0.0f;
        static AccessTools.FieldRef<AutoEngineerPlanner, Orders> _ordersRef = AccessTools.FieldRefAccess<AutoEngineerPlanner, Orders>("_orders");

        private float deaden(float val)
        {
            var result = val;
            if (val < 0.05f)
            {
                result = 0.0f;
            }
            if (val > 0.95f)
            {
                result = 1.0f;
            }
            return result;
        }
        public Ticker()
        {
        }

        private void ChangeValue(BaseLocomotive loco, PropertyChange.Control control, int value)
        {
            StateManager.ApplyLocal(new PropertyChange(loco.id, control, value));
        }

        private Value GetValue(BaseLocomotive loco, PropertyChange.Control control)
        {
            return loco.KeyValueObject[PropertyChange.KeyForControl(control)];
        }

        void Update()
        {
            if (context != null)
            {
                var raildriver = input;
                if (raildriver != null && raildriver.Connected)
                {
                    arity++;
                    var controller = TrainController.Shared;
                    if (controller != null && controller.SelectedLocomotive is BaseLocomotive loco)
                    {
                        bool ignore_input = false;
                        if (raildriver.Wipers < 0.7)
                        {
                            raildriver.UpdateDisplay("OFF");
                            ignore_input = true;
                        }
                        else if (controller.SelectedLocomotive.AutoEngineerPlanner != null)
                        {
                            Orders orders = _ordersRef(controller.SelectedLocomotive.AutoEngineerPlanner);
                            if(orders.Enabled == true)
                            {
                                raildriver.UpdateDisplay("AE ");
                                ignore_input = true;
                            }
                        }

                        if (!ignore_input)
                        {
                            raildriver.UpdateVelocityDisplay(Math.Abs(loco.velocity));
                            trainBrake += 1.0f - raildriver.AutoBrake;
                            indBrake += 1.0f - raildriver.IndBrake;
                            if (arity % 10 == 0)
                            {
                                loco.ControlHelper.Reverser = -raildriver.Reverser;
                                loco.ControlHelper.Throttle = deaden(raildriver.Throttle);
                                loco.ControlHelper.TrainBrake = deaden(trainBrake / 10.0f);
                                loco.ControlHelper.LocomotiveBrake = deaden(indBrake / 10.0f);
                                trainBrake = 0.0f;
                                indBrake = 0.0f;
                            }

                            if (raildriver.BailOff > 0.7f)
                                loco.ControlHelper.BailOff();

                            int headlight = (int)Math.Round(raildriver.Lights * 2.0f);
                            int bits = HeadlightStateLogic.IntFromStates((HeadlightController.State)headlight, (HeadlightController.State)headlight);
                            if (bits != GetValue(loco, PropertyChange.Control.Headlight).IntValue)
                            {
                                ChangeValue(loco, PropertyChange.Control.Headlight, bits);
                            }

                            ButtonMask downMask = raildriver.Buttons;
                            ButtonMask pressed = downMask & (downMask ^ prevMask);

                            if (pressed.HasFlag(ButtonMask.Bell))
                            {
                                loco.ControlHelper.Bell = !loco.ControlHelper.Bell;
                            }

                            float whistle = 0.0f;
                            if (downMask.HasFlag(ButtonMask.WhistleUp))
                                whistle += 1.0f;
                            if (downMask.HasFlag(ButtonMask.WhistleDown))
                                whistle += 0.5f;

                            loco.ControlHelper.Horn = whistle;

                            prevMask = downMask;
                        }
                    }
                    else
                    {
                        raildriver.UpdateDisplay("LOC");
                    }
                }
                // context.Log.Info("AAAAAAAAAAAAAAAAAAA");
            }
        }
    }
}