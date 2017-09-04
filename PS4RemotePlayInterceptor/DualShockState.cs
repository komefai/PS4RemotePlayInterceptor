using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PS4RemotePlayInterceptor
{
    // References
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Device.cs
    // http://www.psdevwiki.com/ps4/DS4-USB

    public class DualShockState
    {
        private enum VK : byte
        {
            L2 = 1 << 2,
            R2 = 1 << 3,
            Triangle = 1 << 7,
            Circle = 1 << 6,
            Cross = 1 << 5,
            Square = 1 << 4,
            DPad_Up = 1 << 3,
            DPad_Down = 1 << 2,
            DPad_Left = 1 << 1,
            DPad_Right = 1 << 0,
            L1 = 1 << 0,
            R1 = 1 << 1,
            Share = 1 << 4,
            Options = 1 << 5,
            L3 = 1 << 6,
            R3 = 1 << 7,
            PS = 1 << 0,
            TouchButton = 1 << 2 - 1
        }

        public byte LX { get; set; }
        public byte LY { get; set; }
        public byte RX { get; set; }
        public byte RY { get; set; }
        public byte L2 { get; set; }
        public byte R2 { get; set; }
        public bool Triangle { get; set; }
        public bool Circle { get; set; }
        public bool Cross { get; set; }
        public bool Square { get; set; }
        public bool DPad_Up { get; set; }
        public bool DPad_Down { get; set; }
        public bool DPad_Left { get; set; }
        public bool DPad_Right { get; set; }
        public bool L1 { get; set; }
        public bool R1 { get; set; }
        public bool Share { get; set; }
        public bool Options { get; set; }
        public bool L3 { get; set; }
        public bool R3 { get; set; }
        public bool PS { get; set; }
        public bool TouchButton { get; set; }

        public byte FrameCounter { get; set; }

        /* Constructor */
        public DualShockState()
        {
            LX = 0x80;
            LY = 0x80;
            RX = 0x80;
            RY = 0x80;
        }

        public static DualShockState ParseFromDualshockRaw(byte[] data)
        {
            // Validate data
            if (data == null)
                return null;
            if (data[0] != 0x1)
                return null;

            // Create empty state to fill in data
            var result = new DualShockState();

            result.LX = data[1];
            result.LY = data[2];
            result.RX = data[3];
            result.RY = data[4];
            result.L2 = data[8];
            result.R2 = data[9];
            result.Triangle = (data[5] & (byte)VK.Triangle) != 0;
            result.Circle = (data[5] & (byte)VK.Circle) != 0;
            result.Cross = (data[5] & (byte)VK.Cross) != 0;
            result.Square = (data[5] & (byte)VK.Square) != 0;
            result.DPad_Up = (data[5] & (byte)VK.DPad_Up) != 0;
            result.DPad_Down = (data[5] & (byte)VK.DPad_Down) != 0;
            result.DPad_Left = (data[5] & (byte)VK.DPad_Left) != 0;
            result.DPad_Right = (data[5] & (byte)VK.DPad_Right) != 0;

            //Convert dpad into individual On/Off bits instead of a clock representation
            byte dpadState = 0;

            dpadState = (byte)(
            ((result.DPad_Right ? 1 : 0) << 0) |
            ((result.DPad_Left ? 1 : 0) << 1) |
            ((result.DPad_Down ? 1 : 0) << 2) |
            ((result.DPad_Up ? 1 : 0) << 3));

            switch (dpadState)
            {
                case 0: result.DPad_Up = true; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = false; break; // ↑
                case 1: result.DPad_Up = true; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = true; break; // ↑→
                case 2: result.DPad_Up = false; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = true; break; // →
                case 3: result.DPad_Up = false; result.DPad_Down = true; result.DPad_Left = false; result.DPad_Right = true; break; // ↓→
                case 4: result.DPad_Up = false; result.DPad_Down = true; result.DPad_Left = false; result.DPad_Right = false; break; // ↓
                case 5: result.DPad_Up = false; result.DPad_Down = true; result.DPad_Left = true; result.DPad_Right = false; break; // ↓←
                case 6: result.DPad_Up = false; result.DPad_Down = false; result.DPad_Left = true; result.DPad_Right = false; break; // ←
                case 7: result.DPad_Up = true; result.DPad_Down = false; result.DPad_Left = true; result.DPad_Right = false; break; // ↑←
                case 8: result.DPad_Up = false; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = false; break; // -
            }

            bool L2Pressed = (data[6] & (byte)VK.L2) != 0;
            bool R2Pressed = (data[6] & (byte)VK.R2) != 0;
            result.L1 = (data[6] & (byte)VK.L1) != 0;
            result.R1 = (data[6] & (byte)VK.R1) != 0;
            result.Share = (data[6] & (byte)VK.Share) != 0;
            result.Options = (data[6] & (byte)VK.Options) != 0;
            result.L3 = (data[6] & (byte)VK.L3) != 0;
            result.R3 = (data[6] & (byte)VK.R3) != 0;
            result.PS = (data[7] & (byte)VK.PS) != 0;
            result.TouchButton = (data[7] & (byte)VK.TouchButton) != 0;

            result.FrameCounter = (byte)(data[7] >> 2);

            return result;
        }

        public void ConvertToDualshockRaw(ref byte[] data)
        {
            data[1] = LX;
            data[2] = LY;
            data[3] = RX;
            data[4] = RY;
            data[8] = L2;
            data[9] = R2;

            byte data_5 = 0;
            if (Triangle) { data_5 += (byte)VK.Triangle; }
            if (Circle) { data_5 += (byte)VK.Circle; }
            if (Cross) { data_5 += (byte)VK.Cross; }
            if (Square) { data_5 += (byte)VK.Square; }
            if (DPad_Up && !DPad_Down && !DPad_Left && !DPad_Right) { data_5 += 0; } // ↑
            if (DPad_Up && !DPad_Down && !DPad_Left && DPad_Right) { data_5 += 1; } // ↑→
            if (!DPad_Up && !DPad_Down && !DPad_Left && DPad_Right) { data_5 += 2; } // →
            if (!DPad_Up && DPad_Down && !DPad_Left && DPad_Right) { data_5 += 3; } // ↓→
            if (!DPad_Up && DPad_Down && !DPad_Left && !DPad_Right) { data_5 += 4; } // ↓
            if (!DPad_Up && DPad_Down && DPad_Left && !DPad_Right) { data_5 += 5; } // ↓←
            if (!DPad_Up && !DPad_Down && DPad_Left && !DPad_Right) { data_5 += 6; } // ←
            if (DPad_Up && !DPad_Down && DPad_Left && !DPad_Right) { data_5 += 7; } // ↑←
            if (!DPad_Up && !DPad_Down && !DPad_Left && !DPad_Right) { data_5 += 8; } // -
            data[5] = data_5;

            byte data_6 = 0;
            if (L2 > 0) { data_6 += (byte)VK.L2; }
            if (R2 > 0) { data_6 += (byte)VK.R2; }
            if (L1) { data_6 += (byte)VK.L1; }
            if (R1) { data_6 += (byte)VK.R1; }
            if (Share) { data_6 += (byte)VK.Share; }
            if (Options) { data_6 += (byte)VK.Options; }
            if (L3) { data_6 += (byte)VK.L3; }
            if (R3) { data_6 += (byte)VK.R3; }
            data[6] = data_6;

            byte data_7 = 0;
            if (PS) { data_7 += (byte)VK.PS; }
            if (TouchButton) { data_7 += (byte)VK.TouchButton; }
            byte currentFrameCounter = (byte)(data[7] >> 2);
            data_7 += (byte)(currentFrameCounter << 2);
            data[7] = data_7;
        }

        public static void Serialize(string path, List<DualShockState> list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DualShockState>));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, list);
            }
        }

        public static List<DualShockState> Deserialize(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<DualShockState>));
            using (TextReader reader = new StreamReader(path))
            {
                object obj = deserializer.Deserialize(reader);
                List<DualShockState> list = obj as List<DualShockState>;
                return list;
            }
        }
    }
}
