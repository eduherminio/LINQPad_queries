<Query Kind="Program" />

void Main()
{
	// May be needed to use ISO-8859-8 in certain .NET programs
	// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

	string rawStr = "Energía Invierno proliferación opción, detonación catástrofe paragüero ànims actúa ácrata castaña façade";
	var tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(rawStr);
	string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);

	asciiStr.Dump();
}