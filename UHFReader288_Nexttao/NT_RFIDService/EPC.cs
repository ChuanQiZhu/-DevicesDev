using System.Runtime.Serialization;

namespace NT_RFIDService
{
    [DataContract]
    public class EPC
    {
        public EPC()
        {

        }

        public EPC(string DeviceNo, int LineNo, string Epc, string StartDatetime, string EndDatetime, int totalRSSI, int numRSSI,int maxRSSI, int minRSSI)
        {
            this.DeviceNo = DeviceNo;
            this.LineNo = LineNo;
            this.Epc = Epc;
            this.StartDatetime = StartDatetime;
            this.EndDatetime = EndDatetime;
            this.totalRSSI = totalRSSI;
            this.maxRSSI = maxRSSI;
            this.minRSSI = minRSSI;
        }

        [DataMember(Order = 0, IsRequired = true)]
        public string Epc { get; set; }

        [DataMember(Order = 1)]
        public string StartDatetime { get; set; }

        [DataMember(Order = 2)]
        public string EndDatetime { get; set; }

        [DataMember(Order = 3)]
        private string DeviceNo { get; set; }

        [DataMember(Order = 4)]
        private int LineNo { get; set; }

        [DataMember(Order = 5)]
        private int totalRSSI { get; set; }

        [DataMember(Order = 6)]
        private int numRSSI { get; set; }

        [DataMember(Order = 7)]
        private int maxRSSI { get; set; }

        [DataMember(Order = 8)]
        private int minRSSI { get; set; }
        // private string Epc;
        // private string StartDatetime;
        // private string EndDatetime;


        public string getDeviceNo()
        {
            return DeviceNo;
        }

        public void setDeviceNo(string deviceNo)
        {
            this.DeviceNo = deviceNo;
        }

        public int getLineNo()
        {
            return LineNo;
        }

        public void setLineNo(int LineNo)
        {
            this.LineNo = LineNo;
        }

        public string getEpc()
        {
            return Epc;
        }
        public void setEpc(string epc)
        {
            this.Epc = epc;
        }

        public string getStartDatetime()
        {
            return StartDatetime;
        }

        public void setStartDatetime(string startDatetime)
        {
            this.StartDatetime = startDatetime;
        }

        public string getEndDatetime()
        {
            return EndDatetime;
        }

        public void setEndDatetime(string endDatetime)
        {
            this.EndDatetime = endDatetime;
        }

        public int getTotalRSSI()
        {
            return totalRSSI;
        }

        public void setTotalRSSI(int totalRSSI)
        {
            this.totalRSSI = totalRSSI;
        }

        public int getNumRSSI()
        {
            return numRSSI;
        }

        public void setNumRSSI(int numRSSI)
        {
            this.numRSSI = numRSSI;
        }

        public int getMaxRSSI()
        {
            return maxRSSI;
        }

        public void setMaxRSSI(int maxRSSI)
        {
            this.maxRSSI = maxRSSI;
        }
        public int getMinRSSI()
        {
            return minRSSI;
        }

        public void setMinRSSI(int minRSSI)
        {
            this.minRSSI = minRSSI;
        }

        public string Tostring()
        {

            return "{\"Epc\": \"" + Epc + "\",\"totalRSSI\":\"" + totalRSSI + "\",\"numRSSI\":\"" + numRSSI + "\",\"maxRSSI\":\"" + maxRSSI + "\",\"minRSSI\":\"" + minRSSI + "\",\"StartDatetime\":\"" + StartDatetime + "\",\"EndDatetime\":\"" + EndDatetime + "\"}";
        }
    }
}
