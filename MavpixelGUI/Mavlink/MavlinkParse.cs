using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MavlinkParse
    {
        public int packetcount = 0;

        public int badCRC = 0;
        public int badLength = 0;

        public static void ReadWithTimeout(Stream BaseStream, byte[] buffer, int offset, int count)
        {
            int timeout = BaseStream.ReadTimeout;

            if (timeout == -1)
                timeout = 60000;

            DateTime to = DateTime.Now.AddMilliseconds(timeout);

            int toread = count;
            int pos = offset;

            while (true)
            {
                // read from stream
                int read = BaseStream.Read(buffer, pos, toread);

                // update counter
                toread -= read;
                pos += read;

                // reset timeout if we get data
                if (read > 0)
                    to = DateTime.Now.AddMilliseconds(timeout);

                if (toread == 0)
                    break;

                if (DateTime.Now > to)
                {
                    throw new TimeoutException("Timeout waiting for data");
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        public MAVLinkMessage ReadPacket(Stream BaseStream)
        {
            byte[] buffer = new byte[270];

            int readcount = 0;

            while (readcount < 200)
            {
                // read STX byte
                ReadWithTimeout(BaseStream, buffer, 0, 1);

                if (buffer[0] == MAVLink.MAVLINK_STX || buffer[0] == MAVLINK_STX_MAVLINK1)
                    break;

                readcount++;
            }

            var headerlength = buffer[0] == MAVLINK_STX ? MAVLINK_CORE_HEADER_LEN : MAVLINK_CORE_HEADER_MAVLINK1_LEN;
            var headerlengthstx = headerlength + 1;

            // read header
            ReadWithTimeout(BaseStream, buffer, 1, headerlength);

            // packet length
            int lengthtoread = 0;
            if (buffer[0] == MAVLINK_STX)
            {
                lengthtoread = buffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - magic - length
                if ((buffer[2] & MAVLINK_IFLAG_SIGNED) > 0)
                {
                    lengthtoread += MAVLINK_SIGNATURE_BLOCK_LEN;
                }
            }
            else
            {
                lengthtoread = buffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - U - length    
            }

            //read rest of packet
            ReadWithTimeout(BaseStream, buffer, 6, lengthtoread - (headerlengthstx-2));

            MAVLinkMessage message = new MAVLinkMessage(buffer);

            // resize the packet to the correct length
            Array.Resize<byte>(ref buffer, lengthtoread + 2);

            // calc crc
            ushort crc = MavlinkCRC.crc_calculate(buffer, buffer.Length - 2);

            // calc extra bit of crc for mavlink 1.0+
            if (message.header == MAVLINK_STX || message.header == MAVLINK_STX_MAVLINK1)
            {
                crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[message.msgid], crc);
            }

            // check crc
            if ((message.crc16 >> 8) != (crc >> 8) ||
                      (message.crc16 & 0xff) != (crc & 0xff))
            {
                badCRC++;
                // crc fail
                return null;
            }

            return message;
        }

        //Got a packet event
        public delegate void MavMessageEventHandler(object sender, MavMessageEventArgs e);
        public event MavMessageEventHandler MavMessageHandler;
        public void OnMavMessageData(MAVLinkMessage message)
        {
            if (MavMessageHandler != null) MavMessageHandler(this, new MavMessageEventArgs(message));
        }

        //Packet error event
        public delegate void MavErrorEventHandler(object sender, MavErrorEventArgs e);
        public event MavErrorEventHandler MavErrorHandler;
        public void OnMavError(MavError error)
        {
            if (MavErrorHandler != null) MavErrorHandler(this, new MavErrorEventArgs(error));
        }

        byte[] collectBuffer = new byte[270];
        int collectCount = 0;
        bool collectGotHeader = false;
        public string CollectPacket(byte[] data)
        {
            int dataIndex = 0;
            StringBuilder nonMavData = new StringBuilder();
            while (dataIndex < data.Length)
            {
                byte dataByte = data[dataIndex++];
                if (collectCount == 0)
                {
                    // read STX byte
                    collectBuffer[0] = dataByte;
                    if (collectBuffer[0] == MAVLink.MAVLINK_STX || collectBuffer[0] == MAVLINK_STX_MAVLINK1)
                    {
                        collectCount = 1;
                        collectGotHeader = false;
                    }
                    else nonMavData.Append(Encoding.ASCII.GetString(new byte[] { dataByte }));
                    continue;
                }
                var headerlength = collectBuffer[0] == MAVLINK_STX ? MAVLINK_CORE_HEADER_LEN : MAVLINK_CORE_HEADER_MAVLINK1_LEN;
                var headerlengthstx = headerlength + 1;
                if (!collectGotHeader && collectCount < headerlengthstx)
                {
                    // read header
                    collectBuffer[collectCount++] = dataByte;
                    if (collectCount >= headerlengthstx)
                    {
                        collectGotHeader = true;
                        collectCount = 6;
                    }
                    continue;
                }

                // packet length
                int lengthToRead;
                if (collectBuffer[0] == MAVLINK_STX)
                {
                    lengthToRead = collectBuffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - magic - length
                    if ((collectBuffer[2] & MAVLINK_IFLAG_SIGNED) > 0)
                        lengthToRead += MAVLINK_SIGNATURE_BLOCK_LEN;
                }
                else lengthToRead = collectBuffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - U - length    

                //read rest of packet
                //ReadWithTimeout(BaseStream, buffer, 6, lengthtoread - (headerlengthstx - 2));
                if (collectCount < lengthToRead - (headerlengthstx - 2) + 6)
                {
                    collectBuffer[collectCount++] = dataByte;
                    if (collectCount < lengthToRead - (headerlengthstx - 2) + 6) 
                        continue;
                }

                MAVLinkMessage message = new MAVLinkMessage(collectBuffer);

                // resize the packet to the correct length
                //Array.Resize<byte>(ref collectBuffer, lengthToRead + 2);

                // calc crc
                ushort crc = MavlinkCRC.crc_calculate(collectBuffer, collectCount - 2);

                // calc extra bit of crc for mavlink 1.0+
                if ((message.header == MAVLINK_STX || message.header == MAVLINK_STX_MAVLINK1) &&
                    message.msgid < MAVLINK_MESSAGE_CRCS.Count)
                {
                    crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[message.msgid], crc);
                }

                // check crc
                if ((message.crc16 >> 8) != (crc >> 8) ||
                          (message.crc16 & 0xff) != (crc & 0xff))
                {
                    badCRC++;
                    OnMavError(MavError.BAD_CRC);
                }
                else OnMavMessageData(message);
                collectCount = 0;
            }
            return nonMavData.ToString();
        }

        public byte[] GenerateMAVLinkPacket10(MAVLINK_MSG_ID messageType, object indata)
        {
            byte[] data;

            data = MavlinkUtil.StructureToByteArray(indata);

            byte[] packet = new byte[data.Length + 6 + 2];

            packet[0] = 0xfe;
            packet[1] = (byte)data.Length;
            packet[2] = (byte)packetcount;

            packetcount++;

            packet[3] = 255; // this is always 255 - MYGCS
            packet[4] = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
            packet[5] = (byte)messageType;


            int i = 6;
            foreach (byte b in data)
            {
                packet[i] = b;
                i++;
            }

            ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + 6);

            checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[(byte)messageType], checksum);

            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte

            packet[i] = ck_a;
            i += 1;
            packet[i] = ck_b;
            i += 1;

            return packet;
        }
    }
    public class MavMessageEventArgs : EventArgs
    {
        public MAVLinkMessage Message;
        public MavMessageEventArgs(MAVLinkMessage message)
        {
            Message = message;
        }
    }
    public class MavErrorEventArgs : EventArgs
    {
        public MavError Error;
        public MavErrorEventArgs(MavError error)
        {
            Error = error;
        }
    }
    public enum MavError { BAD_CRC };
}
