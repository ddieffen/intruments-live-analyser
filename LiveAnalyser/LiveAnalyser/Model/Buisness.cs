using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Threading;
using LiveAnalyser.Data;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO.Ports;
using System.Speech.Synthesis;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using LiveAnalyser.Controls.WaypointsControls;


namespace LiveAnalyser.Model
{
    internal static class Buisness
    {
        #region private members
        /// <summary>
        /// Used for testing weather or not a COM port is already used and exists before trying to opening it.
        /// User in SelectComPort method
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="securityAttrs"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);
        
        static bool initialized = false;
        static bool pauseDataReading = false;
        static BackgroundWorker fileStreamReader = null;
        static BackgroundWorker comReader = null;
        static BackgroundWorker comLogger = null;
        static Data.DataHolder database = null;
        static SerialPort serialPort = null;
        static string serialPortLogFileName = "";
        static string bufferForOutputFile = "";

        static string latestNMEA = "";
        static double latestTWS = 0;
        static double latestTWD = 0;
        static double latestAWS = 0;
        static double latestAWA = 0;
        static double latestSOG = 0;
        static double latestSOW = 0;
        static double latestHDGT = 0;
        static double latestHDGM = 0;
        static long latestGPSTimeUTC = 0;
        static Position latestPos = new Position();

        static SpeechSynthesizer reader = new SpeechSynthesizer();

        static string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\LiveAnalyser\NMEALogs";

        #endregion

        #region internal delegates and events
        internal delegate void DataReceived(string feed);
        internal static event DataReceived DataReceivedEvent;
        #endregion

        #region internal accessors

        internal static string LatestNMEA
        {
            get { return Buisness.latestNMEA; }
            set { Buisness.latestNMEA = value; }
        }

        internal static double LatestTWS
        {
            get { return Buisness.latestTWS; }
            set { Buisness.latestTWS = value; }
        }

        internal static double LatestTWD
        {
            get { return Buisness.latestTWD; }
            set { Buisness.latestTWD = value; }
        }

        internal static double LatestAWS
        {
            get { return Buisness.latestAWS; }
            set { Buisness.latestAWS = value; }
        }

        internal static double LatestAWA
        {
            get { return Buisness.latestAWA; }
            set { Buisness.latestAWA = value; }
        }

        internal static double LatestSOG
        {
            get { return Buisness.latestSOG; }
            set { Buisness.latestSOG = value; }
        }

        internal static double LatestSOW
        {
            get { return Buisness.latestSOW; }
            set { Buisness.latestSOW = value; }
        }

        internal static double LatestHDGT
        {
            get { return Buisness.latestHDGT; }
            set { Buisness.latestHDGT = value; }
        }

        internal static double LatestHDGM
        {
            get { return Buisness.latestHDGM; }
            set { Buisness.latestHDGM = value; }
        }

        internal static Position LatestPos
        {
            get { return Buisness.latestPos; }
            set { Buisness.latestPos = value; }
        }

        internal static long LatestGPSTimeUTC
        {
            get { return latestGPSTimeUTC; }
        }

        /// <summary>
        /// Returns the database
        /// </summary>
        internal static Data.DataHolder Database
        {
            get
            {
                return database;
            }
        }

        internal static bool IsCOMPortActive
        {
            get 
            {
                return serialPort != null && serialPort.IsOpen;
            }
        }

        internal static bool IsSimulationActive
        {
            get 
            {
                return fileStreamReader != null && fileStreamReader.IsBusy;
            }
        }

        #endregion

        #region internal methods
   
        /// <summary>
        /// Initialize the settings for the Buisness class, only need to be called once at startup
        /// </summary>
        internal static void Initialize()
        {
            if (initialized == false)
            {
                latestGPSTimeUTC = Tools.ToPOSIX(DateTime.Now);
                InitializeBackgroundWorkers();
                InitializeDatabase();
                InitializeFolders();
                try
                {
                    OpenComPort(Properties.Settings.Default.SerialPortName,
                                Properties.Settings.Default.SerialPortBaud,
                                Properties.Settings.Default.SerialPortParity,
                                Properties.Settings.Default.SerialPortDataBits,
                                Properties.Settings.Default.SerialPortStopBits);
                }
                catch { }
            }
            initialized = true;
       }

        /// <summary>
        /// Text to speech for sentence passed as an argument
        /// Only works if Properties.Settings.Default.ActivateSpeech is set to True, otherwise do nothing
        /// </summary>
        /// <param name="toBeSaid"></param>
        internal static void Say(string toBeSaid)
        {
            if(Properties.Settings.Default.ActivateSpeech)
                reader.SpeakAsync(toBeSaid);
        }      
     
        /// <summary>
        /// Start reading a file as a stream to simulate data coming on the serial port
        /// </summary>
        /// <param name="filename"></param>
        internal static void UseFileStreamAsInput(string filename)
        {

            if (serialPort != null)
                serialPort.Close();

            if (fileStreamReader.IsBusy)
                fileStreamReader.CancelAsync();
            if (fileStreamReader.CancellationPending == false)
                fileStreamReader.RunWorkerAsync(filename);

        }
     
        /// <summary>
        /// Called when the application is closing
        /// Destroy the connection to the COM port or the file reader
        /// Saves data and parameters that needs to be saved
        /// </summary>
        internal static void Stopping()
        {

        }
     
        /// <summary>
        /// Initialize the settings for the COM port and opens it
        /// </summary>
        /// <param name="port">Port name</param>
        /// <param name="baud">Baud rate</param>
        /// <param name="parity">Parity</param>
        /// <param name="databits">Data bits</param>
        /// <param name="stopbits">Stop bits</param>
        internal static void OpenComPort(string portName, int baud, Parity parity, int databits, StopBits stopbits)
        {
            DefineNewCOMLogFile();

            if (!fileStreamReader.IsBusy)
            {
                int dwFlagsAndAttributes = 0x40000000;

                var isValid = SerialPort.GetPortNames().Any(x => string.Compare(x, portName, true) == 0);
                if (!isValid)
                    throw new System.IO.IOException(string.Format("{0} port was not found", portName));

                //Borrowed from Microsoft's Serial Port Open Method :)
                SafeFileHandle hFile = CreateFile(@"\\.\" + portName, -1073741824, 0, IntPtr.Zero, 3, dwFlagsAndAttributes, IntPtr.Zero);
                if (hFile.IsInvalid)
                    throw new System.IO.IOException(string.Format("{0} port is already open", portName));

                hFile.Close();

                if (serialPort != null)
                {
                    serialPort.Close();
                    comReader.CancelAsync();
                    serialPort.Dispose();
                }
                serialPort = new SerialPort(portName, baud, parity, databits, stopbits);
                serialPort.Open();
                serialPort.NewLine = "\r\n";
                comReader.RunWorkerAsync();
                Properties.Settings.Default.SerialPortName = portName;
                Properties.Settings.Default.SerialPortBaud = baud;
                Properties.Settings.Default.SerialPortParity = parity;
                Properties.Settings.Default.SerialPortDataBits = databits;
                Properties.Settings.Default.SerialPortStopBits = stopbits;
                Properties.Settings.Default.Save();
            }
            else
                throw new System.Exception("Cannot initialize the COM port while reading an simulation NMEA file");
        }
    
        /// <summary>
        /// Orders the COM port to be closed
        /// </summary>
        internal static void StopComPort()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                comReader.CancelAsync();
            }
        }
      
        /// <summary>
        /// Orders the reading of a file to be stopped
        /// </summary>
        internal static void CancelFileReading()
        {
            if (fileStreamReader != null)
                fileStreamReader.CancelAsync();
        }

        /// <summary>
        /// Saves all the data points (wind, course...) into a compressed xml file
        /// </summary>
        /// <param name="p">Filename to save database in</param>
        /// <returns>True if file was sucessfully saved, false otherwise</returns>
        internal static bool SaveData(string filename)
        {
            try
            {
                MemoryStream savingStream = new MemoryStream();
                MemoryStream dataStream = new MemoryStream();
                XmlSerializer x = new XmlSerializer(Buisness.Database.GetType());
                x.Serialize(dataStream, Buisness.Database);
                Tools.CompDecomp.Compress(dataStream, savingStream);

                FileStream fs = new FileStream("data.bin", FileMode.Create);
                savingStream.Position = 0;
                savingStream.WriteTo(fs);
                fs.Close();
                savingStream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clears and populates the actual database content with a compressed xml file
        /// </summary>
        /// <param name="filename">Filename to load from</param>
        /// <returns>True if file was sucessfully loaded, false otherwise</returns>
        internal static bool LoadData(string filename)
        {
            try
            {

                // Create an instance of the XmlSerializer specifying type and namespace.
                XmlSerializer serializer = new XmlSerializer(typeof(DataHolder));

                // A FileStream is needed to read the XML document.
                FileStream fs = new FileStream(filename, FileMode.Open);
                MemoryStream readableStream = new MemoryStream();
                Tools.CompDecomp.Decompress(fs, readableStream);

                //Saving the decompressed memory stream to a file then open the file apprears to work fine
                readableStream.Position = 0;
                XmlReader reader = new XmlTextReader(readableStream);

                // Use the Deserialize method to restore the object's state.
                database = (DataHolder)serializer.Deserialize(reader);
                readableStream.Close();

                return true;
            }
            catch
            {


                return false;

            }
        }
        #endregion
       
        #region private methods

        static void InitializeBackgroundWorkers()
        {
            fileStreamReader = new BackgroundWorker();
            fileStreamReader.WorkerSupportsCancellation = true;
            fileStreamReader.DoWork += new DoWorkEventHandler(streamReader_DoWork);
            fileStreamReader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(streamReader_RunWorkerCompleted);

            comReader = new BackgroundWorker();
            comReader.WorkerSupportsCancellation = true;
            comReader.DoWork += new DoWorkEventHandler(comReader_DoWork);

            comLogger = new BackgroundWorker();
            comLogger.WorkerSupportsCancellation = true;
            comLogger.DoWork += new DoWorkEventHandler(comLogger_DoWork);
        }

        static void InitializeDatabase()
        {
            database = new Data.DataHolder();
        }

        static void InitializeFolders()
        {
            try { reader.SelectVoice("Microsoft Mary"); }
            catch { }

            if (String.IsNullOrEmpty(Properties.Settings.Default.COMArchivesFolder))
            {
                int safety_int = 100;

                bool logFolderFound = System.IO.Directory.Exists(logFolder);

                while (logFolderFound == false && safety_int > 0)
                {
                    Directory.CreateDirectory(logFolder);
                    logFolderFound = Directory.Exists(logFolder);
                    safety_int--;
                }

                Properties.Settings.Default.COMArchivesFolder = logFolder;
                Properties.Settings.Default.Save();
            }
            else
                logFolder = Properties.Settings.Default.COMArchivesFolder;
        }

        static void DefineNewCOMLogFile()
        {
            if (comLogger != null)
                comLogger.CancelAsync();
            serialPortLogFileName = logFolder + @"\NMEA " + DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH'-'mm'-'ss") + ".log";
        }

        static void comLogger_DoWork(object sender, DoWorkEventArgs e)
        {
            if (comLogger.CancellationPending == false)
            {
                try
                {
                    using (StreamWriter outfile = new StreamWriter(serialPortLogFileName, true))
                    {
                        outfile.WriteLine(bufferForOutputFile.TrimEnd('\r', '\n'));;
                        bufferForOutputFile = "";
                        outfile.Close();
                    }
                }
                catch { }
            }
        }

        static void streamReader_DoWork(object sender, DoWorkEventArgs e)
        {
            String filename = e.Argument as String;
            var filestream = new FileStream(filename,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite);
            var file = new StreamReader(filestream, Encoding.UTF8, true, 128);
            if (file != null)
            {
                string tmp = "";
                string NMEAstring = "";
                int nbrOfLines = 14;
                long time = 0;
                database.Clear();

                Dictionary<long, string> toBeAdded = new Dictionary<long, string>();

                while (tmp != null && !fileStreamReader.CancellationPending)
                {
                    for (int i = 0; i < nbrOfLines; i++)
                    {
                        if ((tmp = file.ReadLine()) == null)
                        {
                            NMEAstring = "";
                            tmp = null;
                            break;
                        }
                        NMEAstring += tmp + System.Environment.NewLine;
                    }

                    int count = 0;
                    while (pauseDataReading)
                    {
                        count++;
                        if (fileStreamReader.CancellationPending)
                            break;
                        Thread.Sleep(10);
                        if (count > 5000)
                            e.Cancel = true;
                        
                    }

                    if (fileStreamReader.CancellationPending)
                        break;

                    toBeAdded.Add(time, NMEAstring); 
                    NMEAstring = "";
                    time += 3;
                }

                //This is modified so when a file is added, it's added in the past of Now
                //Then the plots that are showing the past 15 minuntes or so show something, the previous
                //method of simulation was adding points in the future of now.
                long currentTime = Tools.ToPOSIX(DateTime.Now);
                long latestSentence = toBeAdded.Keys.Max();
                long differece = currentTime - latestSentence;
                foreach (KeyValuePair<long, string> pair in toBeAdded.OrderByDescending(item => item.Key))
                {
                    IncomingData(pair.Value, database, pair.Key + differece, "simulation");
                    Thread.Sleep(3);  /// -don-
                }

                if (fileStreamReader.CancellationPending)
                    e.Cancel = true;
            }
        }

        static void streamReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (serialPort != null && !serialPort.IsOpen)
            {
                serialPort.Open();
            }
        }

        static void IncomingData(string NMEAstring, Data.DataHolder data, long time, string feed)
        {
            Buisness.latestNMEA = NMEAstring;
            FindWeatherElements(NMEAstring, time, data);
            FindCourseElements(NMEAstring, time, data);
            if (DataReceivedEvent != null)
                DataReceivedEvent(feed);

            bufferForOutputFile += NMEAstring;
            if (comLogger != null && comLogger.IsBusy == false)
                comLogger.RunWorkerAsync(bufferForOutputFile);
        }

        /// <summary>
        /// Creates data elements from wind speed and wind direction
        /// 
        /// MWD Wind Direction and Speed (TWD °M / °T and TWS) 
        /// $IIMWD,357,T,000,M,8.00,N,4.11,M*49
        /// 
        /// MWV Wind Speed and Angle (AWS and AWA, flag set to R)
        /// $IIMWV,228,T,8.00,N,A*15
        /// </summary>
        /// <param name="NMEAstring"></param>
        /// <param name="data"></param>
        static void FindWeatherElements(string NMEAstring, long time, Data.DataHolder data)
        {
            foreach (var sentence in NMEAstring.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string checksum = getChecksum(sentence);
                string[] split = sentence.Split('*');
                if(split.Count() == 1 || (split.Count() == 2 && split[1] == checksum))
                {//valid sentence
                    string[] dataSplit = split[0].Split(',');
                    if (dataSplit[0].ToLower().Contains("mwd"))
                    {
                        double tempValue;
                        if (dataSplit.Length >= 3 &&
                            Double.TryParse(dataSplit[1], out tempValue)
                            && dataSplit[2].ToLower() == "t"
                            && !data.TWDT.ContainsKey(time))
                        {
                            data.TWDT.TryAdd(time, tempValue);
                            latestTWD = tempValue;
                        }
                        if (dataSplit.Length >= 5 &&
                            Double.TryParse(dataSplit[3], out tempValue)
                            && dataSplit[4].ToLower() == "m"
                            && !data.TWDD.ContainsKey(time))
                        {
                            data.TWDD.TryAdd(time, tempValue); 
                            latestTWD = tempValue;
                        }
                        if (dataSplit.Length >= 7 &&
                            Double.TryParse(dataSplit[5], out tempValue)
                            && dataSplit[6].ToLower() == "n"
                            && !data.TWS.ContainsKey(time))
                        {
                            data.TWS.TryAdd(time, tempValue * 0.514444);
                            latestTWS = tempValue;
                        }
                    }
                    else if (dataSplit[0].ToLower().Contains("mwv"))
                    {
                        double tempValue;
                        if (dataSplit.Length >= 3 &&
                            Double.TryParse(dataSplit[1], out tempValue)
                            && (dataSplit[2].ToLower() == "r") //t is true wind angle corrected for speed
                            && !data.AWA.ContainsKey(time))
                        {
                            data.AWA.TryAdd(time, tempValue);
                            latestAWA = tempValue;
                        }
                        if (dataSplit.Length >= 3 &&
                            Double.TryParse(dataSplit[1], out tempValue)
                            && (dataSplit[2].ToLower() == "t")
                            && !data.TWA.ContainsKey(time))
                        {
                            data.TWA.TryAdd(time, tempValue);
                            latestAWA = tempValue;
                        }
                        if (dataSplit.Length >= 5 &&
                            Double.TryParse(dataSplit[3], out tempValue)
                            && dataSplit[4].ToLower() == "n"
                            && !data.AWS.ContainsKey(time))
                        {
                            data.AWS.TryAdd(time, tempValue * 0.514444);
                            latestAWS = tempValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates data element for course of the boat
        /// 
        /// RMC Recommended minimum specific GNSS data
        /// $IIRMC,183701.00,A,4150.76,N,08733.12,W,6.20,330,,003,W,D*11
        /// 
        /// RMB Recommended minimum navigation information
        /// $IIRMB,A,2.07,R,,WL-MARK2,4150.19,N,08730.43,W,2.09,105,1269.13,V,D*7D
        /// 
        /// VHW Water Speed and Heading (°M / °T)
        /// $IIVHW,322,T,325,M,5.92,N,10.97,K*63
        /// position
        /// $IIGLL,4151.40,N,08734.88,W,140012.00,A,D*62
        /// </summary>
        /// <param name="NMEAstring"></param>
        /// <param name="time"></param>
        /// <param name="data"></param>
        static void FindCourseElements(string NMEAstring, long time, Data.DataHolder data)
        {
            foreach (var sentence in NMEAstring.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string checksum = getChecksum(sentence);
                string[] split = sentence.Split('*');
                if (split.Count() == 1 || (split.Count() == 2 && split[1] == checksum))
                {//valid sentence
                    string[] dataSplit = split[0].Split(',');
                    if (dataSplit[0].ToLower().Contains("vhw"))
                    {
                        double tempValue;
                        if (dataSplit.Length >= 3 &&
                            Double.TryParse(dataSplit[1], out tempValue)
                            && dataSplit[2].ToLower() == "t"
                            && !data.HDGT.ContainsKey(time))
                        {
                            data.HDGT.TryAdd(time, tempValue);
                            latestHDGT = tempValue;
                        }
                        if (dataSplit.Length >= 5 &&
                            Double.TryParse(dataSplit[3], out tempValue)
                            && dataSplit[4].ToLower() == "m"
                            && !data.HDGM.ContainsKey(time))
                        {
                            data.HDGM.TryAdd(time, tempValue);
                            latestHDGM = tempValue;
                        }
                        if (dataSplit.Length >= 7 &&
                            Double.TryParse(dataSplit[5], out tempValue)
                            && dataSplit[6].ToLower() == "n"
                            && !data.SOW.ContainsKey(time))
                        {
                            data.SOW.TryAdd(time, tempValue * 0.514444);
                            latestSOW = tempValue * 0.514444;
                        }
                    } 
                    if (dataSplit[0].ToLower().Contains("gll"))
                    {
                        double tempLat;
                        double tempLon;
                        if (Double.TryParse(dataSplit[1], out tempLat)
                            && Double.TryParse(dataSplit[3], out tempLon)
                            && dataSplit[2].ToLower() == "n"
                            && dataSplit[4].ToLower() == "w"
                            && !data.Pos.ContainsKey(time))
                        {
                            Position tempPos = new Position( dataSplit[1], dataSplit[2], dataSplit[3], dataSplit[4]);
                            //Position tempPos = new Position();
                            //tempPos.lat.degrees = tempLat;
                            //tempPos.lat.dir = dataSplit[2].ToLower();
                            //tempPos.lon.degrees = tempLon;
                            //tempPos.lon.dir = dataSplit[4].ToLower();
                            //tempPos.time = time;
                            data.Pos.TryAdd(time, tempPos);
                            latestPos = tempPos;
                        }
                    }
                    if (dataSplit[0].ToLower().Contains("rmc"))
                    {
                        double tempSog;
                        if (Double.TryParse(dataSplit[1], out tempSog)
                            && !data.SOG.ContainsKey(time))
                        {
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            try
                            {
                                DateTime previousTime = Tools.FromPOSIX(latestGPSTimeUTC);
                                DateTime GpsTime = DateTime.ParseExact(dataSplit[1], "HHmmss.ff", provider);
                                //just update the Time, keep the previously known date
                                latestGPSTimeUTC = Tools.ToPOSIX(new DateTime(previousTime.Year, previousTime.Month, previousTime.Day, GpsTime.Hour, GpsTime.Minute, GpsTime.Second, GpsTime.Millisecond));
                            }
                            catch { }
                        }
                        if (Double.TryParse(dataSplit[9], out tempSog)
                            && !data.SOG.ContainsKey(time))
                        {
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            try
                            {
                                DateTime previousTime = Tools.FromPOSIX(latestGPSTimeUTC);
                                Calendar cal = (Calendar)CultureInfo.CurrentCulture.Calendar.Clone();
                                DateTime GpsTime = DateTime.ParseExact(dataSplit[9], "ddMMyy", provider);
                                //just update the Time, keep the previously known date
                                latestGPSTimeUTC = Tools.ToPOSIX(new DateTime(GpsTime.Year, GpsTime.Month, GpsTime.Day, previousTime.Hour, previousTime.Minute, previousTime.Second, previousTime.Millisecond));
                            }
                            catch { }
                        }
                        if (Double.TryParse(dataSplit[7], out tempSog)
                            && !data.SOG.ContainsKey(time))
                        {
                            data.SOG.TryAdd(time, tempSog * 0.514444);
                            latestSOG = tempSog * 0.514444;
                        }
                    }

                }
            }
        }

        public static string getChecksum(string sentence)
        {
            try
            {
                //Start with first Item
                int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);
                // Loop through all chars to get a checksum
                for (int i = sentence.IndexOf('$') + 2; i < sentence.IndexOf('*'); i++)
                {
                    // No. XOR the checksum with this character's value
                    checksum ^= Convert.ToByte(sentence[i]);
                }
                // Return the checksum formatted as a two-character hexadecimal
                return checksum.ToString("X2");
            }
            catch
            {
                return "";
            }
        }

        static void comReader_DoWork(object sender, DoWorkEventArgs e)
        {
            while (comReader.CancellationPending == false)
            {
                if (fileStreamReader.IsBusy == false || comSending == false)
                {
                    if (serialPort != null && serialPort.IsOpen)
                    {
                        try
                        {
                            string nmeaString = serialPort.ReadLine();
                            long time = Tools.ToPOSIX(DateTime.Now);
                            IncomingData(nmeaString, database, time, "COM");
                        }
                        catch { }
                    }
                }
                Thread.Sleep(10);   
            }
        }
        static bool comSending = false;
        public static void COMSendLine(string line)
        {
            string checkSum = getChecksum(line);
            //line = line.Substring(0, line.IndexOf('*')) + "*" + checkSum + "\r\n";
            //string checksum = getChecksum(line);
            comSending = true;

            try
            {
                if (serialPort != null
                    && serialPort.IsOpen)
                {
                    serialPort.WriteLine(line);
                }
                else
                {
                    OpenComPort(Properties.Settings.Default.SerialPortName,
                                Properties.Settings.Default.SerialPortBaud,
                                Properties.Settings.Default.SerialPortParity,
                                Properties.Settings.Default.SerialPortDataBits,
                                Properties.Settings.Default.SerialPortStopBits);
                    serialPort.WriteLine(line);
                }
            }
            catch (Exception ex)
            { }

            comSending = false;
        }
        #endregion

    }
}
