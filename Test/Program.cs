// See https://aka.ms/new-console-template for more information
using DustAnalyzerSquare;

Console.WriteLine("Hello, World!");
IDustAnalyzerSquare dustAnalyzerSquare = new DustAnalyzerSquare.DustAnalyzerSquare(new Communication.Bus.PhysicalPort.SerialPort("COM3", 38400));
await dustAnalyzerSquare.OpenAsync();
var rs = await dustAnalyzerSquare.Read("01");

Console.ReadLine();