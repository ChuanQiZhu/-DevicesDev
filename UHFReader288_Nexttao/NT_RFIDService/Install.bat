%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\NT_RFIDService_0\NT_RFIDService.exe

Net Start NT_RFIDService

sc config NT_RFIDService start= auto
