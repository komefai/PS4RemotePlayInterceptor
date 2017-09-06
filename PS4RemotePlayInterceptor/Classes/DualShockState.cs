// PS4RemotePlayInterceptor (File: Classes/DualShockState.cs)
//
// Copyright (c) 2017 Komefai
//
// Visit http://komefai.com for more information
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Sixaxis.cs
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Touchpad.cs
    // http://www.psdevwiki.com/ps4/DS4-USB

    public class Touch
    {
        public byte TouchID { get; set; }
        public bool IsTouched { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        /* Constructors */
        public Touch() { }
        public Touch(byte touchID, bool isTouched, int x, int y)
        {
            TouchID = touchID;
            IsTouched = isTouched;
            X = x;
            Y = y;
        }
    }

    public class DualShockState
    {
        public const int TOUCHPAD_DATA_OFFSET = 35;

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

        public DateTime ReportTimeStamp { get; set; }
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
        public byte TouchPacketCounter { get; set; }

        public byte FrameCounter { get; set; }
        public byte Battery { get; set; }
        public bool IsCharging { get; set; }

        public short AccelX { get; set; }
        public short AccelY { get; set; }
        public short AccelZ { get; set; }
        public short GyroX { get; set; }
        public short GyroY { get; set; }
        public short GyroZ { get; set; }

        public Touch Touch1 { get; set; }
        public Touch Touch2 { get; set; }

        private static byte priorInputReport30 = 0xff;

        /* Constructor */
        public DualShockState()
        {
            LX = 0x80;
            LY = 0x80;
            RX = 0x80;
            RY = 0x80;
            FrameCounter = 255; // null
            TouchPacketCounter = 255; // null
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

            result.ReportTimeStamp = DateTime.UtcNow; // timestamp with UTC in case system time zone changes

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

            // Charging/Battery
            try
            {
                result.IsCharging = (data[30] & 0x10) != 0;
                result.Battery = (byte)((data[30] & 0x0F) * 10);

                if (data[30] != priorInputReport30)
                    priorInputReport30 = data[30];
            }
            catch { throw new InterceptorException("Index out of bounds: battery"); }

            // Accelerometer
            byte[] accel = new byte[6];
            Array.Copy(data, 14, accel, 0, 6);
            result.AccelX = (short)((ushort)(accel[2] << 8) | accel[3]);
            result.AccelY = (short)((ushort)(accel[0] << 8) | accel[1]);
            result.AccelZ = (short)((ushort)(accel[4] << 8) | accel[5]);

            // Gyro
            byte[] gyro = new byte[6];
            Array.Copy(data, 20, gyro, 0, 6);
            result.GyroX = (short)((ushort)(gyro[0] << 8) | gyro[1]);
            result.GyroY = (short)((ushort)(gyro[2] << 8) | gyro[3]);
            result.GyroZ = (short)((ushort)(gyro[4] << 8) | gyro[5]);

            try
            {
                for (int touches = data[-1 + TOUCHPAD_DATA_OFFSET - 1], touchOffset = 0; touches > 0; touches--, touchOffset += 9)
                {
                    //bool touchLeft = (data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255) >= 1920 * 2 / 5) ? false : true;
                    //bool touchRight = (data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255) < 1920 * 2 / 5) ? false : true;

                    byte touchID1 = (byte)(data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                    byte touchID2 = (byte)(data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                    bool isTouch1 = (data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] >> 7) != 0 ? false : true; // >= 1 touch detected
                    bool isTouch2 = (data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] >> 7) != 0 ? false : true; // 2 touches detected
                    int currentX1 = data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255);
                    int currentY1 = ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0xF0) >> 4) + (data[3 + TOUCHPAD_DATA_OFFSET + touchOffset] * 16);
                    int currentX2 = data[5 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[6 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255);
                    int currentY2 = ((data[6 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0xF0) >> 4) + (data[7 + TOUCHPAD_DATA_OFFSET + touchOffset] * 16);

                    result.TouchPacketCounter = data[-1 + TOUCHPAD_DATA_OFFSET + touchOffset];
                    result.Touch1 = new Touch(touchID1, isTouch1, currentX1, currentY1);
                    result.Touch2 = new Touch(touchID2, isTouch2, currentX2, currentY2);
                }
            }
            catch { throw new InterceptorException("Index out of bounds: touchpad"); }

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

            // Accelerometer
            data[14] = (byte)(AccelY >> 8);
            data[15] = (byte)(AccelY & 0xFF);

            data[16] = (byte)(AccelX >> 8);
            data[17] = (byte)(AccelX & 0xFF);

            data[18] = (byte)(AccelZ >> 8);
            data[19] = (byte)(AccelZ & 0xFF);

            // Gyro
            data[20] = (byte)(GyroX >> 8);
            data[21] = (byte)(GyroX & 0xFF);

            data[22] = (byte)(GyroY >> 8);
            data[23] = (byte)(GyroY & 0xFF);

            data[24] = (byte)(GyroZ >> 8);
            data[26] = (byte)(GyroZ & 0xFF);

            // Charging/Battery
            byte data_30 = 0;
            if (IsCharging) data_30 += 0x10;
            if (Battery > 0) data_30 += (byte)(Battery / 10);
            data[30] = data_30;

            // Touches
            try
            {
                for (int touches = data[-1 + TOUCHPAD_DATA_OFFSET - 1], touchOffset = 0; touches > 0; touches--, touchOffset += 9)
                {
                    // Ignore
                    // data[-1 + TOUCHPAD_DATA_OFFSET + touchOffset] = TouchPacketCounter;

                    // >= 1 Finger
                    if (Touch1 != null)
                    {
                        byte oldTouchID = (byte)(data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                        if (Touch1.IsTouched)
                            data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] = oldTouchID;

                        var x = Touch1.X;
                        var xRemain = (int)(x / 255);
                        var xLeft = x - (xRemain * 255);
                        var y = Touch1.Y;
                        var yRemain = (int)(y / 16);
                        var yLeft = y - (yRemain * 16);

                        data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)xLeft;
                        data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)(xRemain + (yLeft << 4));
                        data[3 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)yRemain;
                    }

                    // 2 Fingers
                    if (Touch2 != null)
                    {
                        byte oldTouchID = (byte)(data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                        if (Touch2.IsTouched)
                            data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] = oldTouchID;

                        var x = Touch2.X;
                        var xRemain = (int)(x / 255);
                        var xLeft = x - (xRemain * 255);
                        var y = Touch2.Y;
                        var yRemain = (int)(y / 16);
                        var yLeft = y - (yRemain * 16);

                        data[5 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)xLeft;
                        data[6 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)(xRemain + (yLeft << 4));
                        data[7 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)yRemain;
                    }
                }
            }
            catch { }
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
