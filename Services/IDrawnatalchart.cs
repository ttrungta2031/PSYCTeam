using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Services
{
    public interface IDrawnatalchart
    {
        Image Base64ToImage(Image base64String);
        Image GetChart(DateTime birthDate, double longtitude, double latitude);
        string GetChartFile(DateTime birthDate, double longtitude, double latitude);
        string GetChartLinkFirebase(DateTime birthDate, double longtitude, double latitude);
        string GetImageFile(Image base64String);
        string GetImageLinkFirebase(Image base64String);
        (Dictionary<string, (double X, double Y, string planetName)> planetPos, double diff) GetPlanetCoordinate(DateTime birthDate, double longtitude, double latitude);
        Task<string> UploadChart(Stream image, string name);
        Task<string> UploadImage(Stream image, string name);
    }
}