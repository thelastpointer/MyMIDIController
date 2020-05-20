using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;

namespace MyMIDIController
{
    // Uses DryWetMidi: https://github.com/melanchall/drywetmidi
    public partial class Form1 : Form
    {
        private const string DEVICE_NAME = "LPD8";

        public bool Listening
        {
            get { return listening; }
            set
            {
                listening = value;

                label1.Visible = listening;
                button1.Visible = !listening;
            }
        }

        private bool listening = false;
        private InputDevice input;
        private VolumeSetter volume;
        private bool reallyQuit = false;
        private bool settingCheckbox = true;

        public Form1()
        {
            InitializeComponent();
            InitializeMIDI();

            settingCheckbox = true;
            checkBox1.Checked = RunAtStartup.IsEnabled;
            settingCheckbox = false;

            volume = new VolumeSetter();
            volume.Initialize();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!reallyQuit)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShutdownMIDI();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            reallyQuit = true;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!settingCheckbox)
                RunAtStartup.IsEnabled = checkBox1.Checked;
        }

        private void InitializeMIDI()
        {
            Listening = false;

            try
            {
                input = InputDevice.GetByName(DEVICE_NAME);
                {
                    input.EventReceived += OnMIDIEvent;
                    input.StartEventsListening();

                    Listening = true;
                }
            }
            catch
            {
                // Device not found
                Listening = false;
            }
        }

        private void ShutdownMIDI()
        {
            Listening = false;

            if (input != null)
            {
                if (input.IsListeningForEvents)
                {
                    input.StopEventsListening();
                }

                input.Dispose();
                input = null;
            }
        }

        private void OnMIDIEvent(object sender, MidiEventReceivedEventArgs e)
        {
            //e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.
            if (e.Event.EventType == MidiEventType.NoteOn)
            {
                var noteOn = (e.Event as NoteOnEvent);
                // Pad 1
                if (noteOn.NoteNumber == 36)
                {
                    // Play / pause
                    SendMediaKey.SendKeyPress(SendMediaKey.KeyCode.MEDIA_PLAY_PAUSE);
                }
                else if (noteOn.NoteNumber == 37)
                {
                    // Previous
                    SendMediaKey.SendKeyPress(SendMediaKey.KeyCode.MEDIA_PREV_TRACK);
                }
                else if (noteOn.NoteNumber == 38)
                {
                    // Next
                    SendMediaKey.SendKeyPress(SendMediaKey.KeyCode.MEDIA_NEXT_TRACK);
                }
            }
            else if (e.Event.EventType == MidiEventType.ControlChange)
            {
                var control = (e.Event as ControlChangeEvent);
                if (control.ControlNumber == 5)
                {
                    // Volume
                    float value = (float)(byte)control.ControlValue / (float)(byte)SevenBitNumber.MaxValue;
                    volume.Set(value);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!listening)
                InitializeMIDI();
        }
    }
}
