using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace wlkReader
{
    public class clsWeatherData
    {

        //====================================
        // Properties
        //====================================

        public byte dataType = 1;
        public byte archiveInterval;      // number of minutes in the archive
                                          // see below for more details about these next two fields)
        public byte iconFlags;            // Icon associated with this record, plus Edit flags
        public byte moreFlags;            // Tx Id, etc.

        public short packedTime;          // minutes past midnight of the end of the archive period
        public short outsideTemp;         // tenths of a degree F
        public short hiOutsideTemp;       // tenths of a degree F
        public short lowOutsideTemp;      // tenths of a degree F
        public short insideTemp;          // tenths of a degree F
        public short barometer;           // thousandths of an inch Hg
        public short outsideHum;          // tenths of a percent
        public short insideHum;           // tenths of a percent
        public ushort rain;               // number of clicks + rain collector type code
        public short hiRainRate;          // clicks per hour
        public short windSpeed;           // tenths of an MPH
        public short hiWindSpeed;         // tenths of an MPH
        public byte windDirection;        // direction code (0-15, 255)
        public byte hiWindDirection;      // direction code (0-15, 255)
        public short numWindSamples;      // number of valid ISS packets containing wind data
                                          // this is a good indication of reception 
        public short solarRad, hisolarRad;// Watts per meter squared 
        public byte UV, hiUV;            // tenth of a UV Index

        public byte[] leafTemp;          // 4 bytes (whole degrees F) + 90

        public short extraRad;            // used to calculate extra heating effects of the sun in THSW index

        public short[] newSensors;       // 6 bytes reserved for future use
        public byte forecast;            // forecast code during the archive interval

        public byte ET;                  // in thousandths of an inch

        public byte[] soilTemp;          // 6 bytes (whole degrees F) + 90
        public byte[] soilMoisture;      // 6 bytes centibars of dryness
        public byte[] leafWetness;       // 4 bytes Leaf Wetness code (0-15, 255)
        public byte[] extraTemp;         // 7 bytes (whole degrees F) + 90
        public byte[] extraHum;          // 7 bytes whole percent

        public DateTime dteTime;         // full time for this record (not part of the record in the file - set externally).

        //====================================
        // Methods
        //====================================

        //-------------------------------------------------------
        //  ReadRecord() - read 87 byte record into fields of class
        //                  note: first byte of 88 byte record will have been read to ID the record type!
        //-------------------------------------------------------
        public void ReadRecord(BinaryReader brInput)
        {

            archiveInterval = brInput.ReadByte();
            iconFlags = brInput.ReadByte();
            moreFlags = brInput.ReadByte();
            packedTime = brInput.ReadInt16();
            outsideTemp = brInput.ReadInt16();
            hiOutsideTemp = brInput.ReadInt16();
            lowOutsideTemp = brInput.ReadInt16();
            insideTemp = brInput.ReadInt16();
            barometer = brInput.ReadInt16();
            outsideHum = brInput.ReadInt16();
            insideHum = brInput.ReadInt16();
            rain = brInput.ReadUInt16();
            hiRainRate = brInput.ReadInt16();
            windSpeed = brInput.ReadInt16();
            hiWindSpeed = brInput.ReadInt16();
            windDirection = brInput.ReadByte();
            hiWindDirection = brInput.ReadByte();
            numWindSamples = brInput.ReadInt16();
            solarRad = brInput.ReadInt16();
            hisolarRad = brInput.ReadInt16();
            UV = brInput.ReadByte();
            hiUV = brInput.ReadByte();
            leafTemp = brInput.ReadBytes(4);
            extraRad = brInput.ReadInt16();

            newSensors = new short[6];
            for(int i = 0; i< 6; i++)
            {
                newSensors[i] = brInput.ReadInt16();
            }

            forecast = brInput.ReadByte();
            ET = brInput.ReadByte();

            soilTemp = brInput.ReadBytes(6);
            soilMoisture = brInput.ReadBytes(6);
            leafWetness = brInput.ReadBytes(4);
            extraTemp = brInput.ReadBytes(7);
            extraHum = brInput.ReadBytes(7);

        } // end of Decode

    } // end of class clsWeatherData
}
