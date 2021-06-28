using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localicer
{
    public static class ChunkFileManipulation
    {
        private static Encoding enc = new TestEncoding();

        /*
         Base code for reading/writing was written by VNNCC for his localization editor, Github: https://github.com/VNNCC/FbLocalization    
         I heavily modified it to make it easier to understand and perform better
        */

        public static void WriteChunkFile(Stream stream, List<Entry> entries)
        {
            BinaryWriter writer = new BinaryWriter(stream, enc);
            writer.BaseStream.Position = 0;

            writer.Write(0x39000);

            // File size
            writer.Write(0);
            // Write list size
            writer.Write((uint)(entries.Count));

            writer.Write(0);

            // Write temp strings offset
            writer.Write(0);
            WriteCString(writer, "Global");

            // Write zero bytes
            writer.Write(new byte[113]);
            uint dataOffset = (uint)writer.BaseStream.Position;

            writer.BaseStream.Position = 12;
            writer.Write(dataOffset); // Write data offset
            writer.BaseStream.Position = dataOffset;

            writer.Write(new byte[8]);

            uint stringsOffsetStart = (uint)(entries.Count * 8 + writer.BaseStream.Position);
            uint currentStringPosition = stringsOffsetStart;

            uint currentHashOffsetPosition = (uint)writer.BaseStream.Position;

            foreach (Entry entry in entries)
            {
                writer.Write(entry.Hash);
                currentHashOffsetPosition = (uint)writer.BaseStream.Position;

                writer.BaseStream.Position = currentStringPosition;
                uint offset = (uint)writer.BaseStream.Position - stringsOffsetStart;
                WriteCString(writer, entry.Value);
                currentStringPosition = (uint)writer.BaseStream.Position;

                writer.BaseStream.Position = currentHashOffsetPosition;
                writer.Write(offset);
            }

            writer.BaseStream.Position = 4;
            writer.Write((uint)writer.BaseStream.Length - 8);

            writer.BaseStream.Position = 16;
            writer.Write((uint)stringsOffsetStart - 8);

            writer.Close();
        }

        private static void WriteCString(BinaryWriter writer, string text)
        {
            byte[] bytes = GetCBytes(text);
            writer.Write(bytes, 0, bytes.Length);
        }

        private static byte[] GetCBytes(string text)
        {
            List<byte> stringBytes = enc.GetBytes(text).ToList();
            stringBytes.Add((byte)0);
            return stringBytes.ToArray();
        }

        public static List<Entry> ReadChunkFile(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream, enc);
            List<Entry> entries = new List<Entry>();

            reader.BaseStream.Position = 0;

            if (reader.ReadUInt32() != 0x39000)
                throw new InvalidDataException();

            uint fileSize = reader.ReadUInt32();
            uint listSize = reader.ReadUInt32();
            uint dataOffset = reader.ReadUInt32() + 8;
            uint stringsOffset = reader.ReadUInt32() + 8;

            ReadCString(reader);

            var hashList = new List<uint>();
            var offsetList = new List<uint>();

            for(int i = 0; i <= listSize; i++)
            {
                reader.BaseStream.Position = dataOffset;
                uint hash = reader.ReadUInt32();
                uint offset = reader.ReadUInt32();
                dataOffset += 8;

                if (stringsOffset + offset > reader.BaseStream.Length) continue;

                reader.BaseStream.Position = stringsOffset + offset;
                string value = ReadCString(reader);
                entries.Add(new Entry(hash, value));
            }

            reader.Close();
            return entries;
        }

        private static string ReadCString(BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();

            while (true)
            {
                byte c = reader.ReadByte();

                if (c == 0)
                {
                    break;
                }

                bytes.Add(c); 
            }

            return enc.GetString(bytes.ToArray());
        }   
    }

    public class Entry
    {
        public uint Hash;
        public string Value;

        public Entry(uint Hash, string Value)
        {
            this.Hash = Hash;
            this.Value = Value;
        }
    }

    public class TestEncoding : Encoding
    {
        private char[] conversionArray = new[] {
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\n', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', ' ', '!', '"', '#', '&', '%', '&', '\'',
            '(', ')', '*', '+', ',', '-', '.', '/', '0', '1',
            '2', '3', '4', '5', '6', '7', '8', '9', ':', ';',
            '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E',
            'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
            'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c',
            'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
            'x', 'y', 'z', '{', '|', '}', '~', '\0', '\0', 'о',
            'е', 'а', 'и', 'т', 'н', 'р', 'с', 'в', 'О', 'И',
            'Е', 'л', 'д', 'к', 'А', 'Н', 'м', 'у', 'Т', 'Р',
            'п', 'С', 'ы', 'я', 'б', 'ь', 'В', 'з', 'Д', 'К',
            ' ', 'П', 'й', 'Л', 'г', 'ж', 'У', 'ч', 'М', '©',
            'З', '«', 'Я', 'ю', '\0', 'Й', 'х', 'Ы', 'Б', 'ш',
            'ц', 'Г', 'щ', 'Ь', 'Ч', 'Ж', 'Ш', '»', 'Х', 'Ц',
            'Щ', 'Э', 'э', 'ф', 'Ю', 'Ф', '—', '™', '\0', '\0',
            'Ё', 'Ъ', 'ё', '\0', '€', '\"', '\"', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0' };

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return chars.Length;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (var i = 0; i < charCount; i++)
            {
                bytes[byteIndex + i] = GetByte(chars[charIndex + i]);
            }

            return charCount;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return bytes.Length;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (var i = 0; i < byteCount; i++)
            {
                chars[charIndex + i] = conversionArray[bytes[byteIndex + i]];
            }

            return byteCount;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        private byte GetByte(char c)
        {
            for (var i = 0; i < conversionArray.Length; i++)
            {
                if (conversionArray[i] == c)
                    return (byte)i;
            }

            throw new Exception("Invalid character");
        }

        private char GetChar(byte b)
        {
            return conversionArray[b];
        }
    }
}
