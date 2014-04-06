using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LiveAnalyser.Controls.WaypointsControls;


namespace LiveAnalyser.Data
{
    [Serializable]
    public class DataHolder
    {
        /// <summary>
        /// Wind speed in meters per second by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> TWS = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Wind direction relative to the true north by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> TWDT = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Wind speed in meters per second by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> AWS = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Wind direction relative to the true north by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> AWA = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Wind direction relative to the true north by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> TWA = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Wind direction relative to the true north by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> TWDD = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Boat speed over water from water sensor in meters per seconds by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> SOW = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Boat speed over ground from GPS in meters per seconds by time received
        /// </summary>
        public SerializableConcuentDictionary<long, double> SOG = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Boat heading by time recieved
        /// </summary>
        public SerializableConcuentDictionary<long, double> HDGT = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Boat heading by time recieved
        /// </summary>
        public SerializableConcuentDictionary<long, double> HDGM = new SerializableConcuentDictionary<long, double>();
        /// <summary>
        /// Positions by time received
        /// </summary>
        public SerializableConcuentDictionary<long, Position> Pos = new SerializableConcuentDictionary<long, Position>();

        public void Clear()
        {
            SOG = new SerializableConcuentDictionary<long, double>();
            SOW = new SerializableConcuentDictionary<long, double>();
            HDGT = new SerializableConcuentDictionary<long, double>();
            HDGM = new SerializableConcuentDictionary<long, double>();
            Pos = new SerializableConcuentDictionary<long, Position>();
            TWS = new SerializableConcuentDictionary<long, double>();
            TWDT = new SerializableConcuentDictionary<long, double>();
            TWDD = new SerializableConcuentDictionary<long, double>();
            AWS = new SerializableConcuentDictionary<long, double>();
            AWA = new SerializableConcuentDictionary<long, double>();
            TWA = new SerializableConcuentDictionary<long, double>();
        }
    }
}
