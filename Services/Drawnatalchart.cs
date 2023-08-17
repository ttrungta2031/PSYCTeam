using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using SwissEphNet;


namespace PsychologicalCounseling.Services
{
    public class Drawnatalchart : IDrawnatalchart
    {
        private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

        private static string Bucket = "psycteamv1.appspot.com";
        private static string AuthEmail = "tuanninh655@gmail.com";
        private static string AuthPassword = "bivsan1";


        public static Dictionary<string, int> PLANET_ID = new Dictionary<string, int>() //bad design but is PRM391 not SWD391
        {
            {"Sun", 1},
            {"Moon", 2},
            {"Mercury", 3},
            {"Venus", 4},
            {"Mars", 5},
            {"Jupiter", 6},
            {"Saturn", 7},
            {"Uranus", 8},
            {"Neptune", 9},
            {"Pluto", 10},
            {"Earth", 11}
        };
        public static (int X, int Y) center = (319, 317);

        public static double planetRadius = 220D;

        public static double pointRadius = 240D;

       // public static int houseOutterRadius = 160;
        public static int houseOutterRadius = 240;
      //  public static int houseInnerRadius = 120;
        public static int houseInnerRadius = 160;


        private static readonly (int, int) BlueSpecialScope1 = (55, 65);
        private static readonly (int, int) BlueSpecialScope2 = (111, 129);

        private static readonly (int, int) RedSpecialScope1 = (0, 9);
        private static readonly (int, int) RedSpecialScope2 = (81, 99);
        private static readonly (int, int) RedSpecialScope3 = (171, 180);

        private static readonly (int, int) GraySpecialScope1 = (43, 48);
        private static readonly (int, int) GraySpecialScope2 = (133, 137);

        private static readonly Pen BluePen = new(Color.Blue, 1);
        private static readonly Pen RedPen = new(Color.Red, 1);
        private static readonly Pen GrayPen = new(Color.Yellow, 1);
        private readonly SwissEph _swiss;

        public Drawnatalchart()
        {
            _swiss = new SwissEph();
            _swiss.swe_set_ephe_path(null);
        }

        public (Dictionary<string, (double X, double Y, string planetName)> planetPos, double diff) GetPlanetCoordinate(DateTime birthDate, double longtitude, double latitude)
        {
            Dictionary<string, (double, double, string planetName)> planetPosition = new Dictionary<string, (double, double, string)>();
            string error = "";
            double[] planetPos = new double[2];

            double[] xx = new double[6]; //6 position values: longitude, latitude, distance, *long.speed, lat.speed, dist.speed 
            double julianDay = _swiss.swe_julday(birthDate.Year, birthDate.Month, birthDate.Day, birthDate.Hour, SwissEph.SE_GREG_CAL);


            double diff = 0d; //the diffirent of zodiac and house
            _swiss.swe_calc_ut(julianDay, SwissEph.SE_ECL_NUT, 0, xx, ref error);
            double eps_true = xx[0];

            for (int planet = SwissEph.SE_SUN; planet <= SwissEph.SE_PLUTO; planet++)
            {
                if (planet == SwissEph.SE_EARTH) continue;

                _swiss.swe_calc_ut(julianDay, planet, SwissEph.SEFLG_SPEED, xx, ref error);
                string planetName = _swiss.swe_get_planet_name(planet);
                planetPosition[planetName] = (-xx[0] + 180, xx[1], planetName.Substring(0, 2).ToLower());

                if (planet == SwissEph.SE_SUN)
                {
                    double[] ascmc = new double[10];
                    double[] cusps = new double[13];
                    _swiss.swe_houses(julianDay, latitude, longtitude, 'A', cusps, ascmc);
                    planetPos[0] = xx[0];
                    planetPos[1] = xx[1];
                    double houseOfPlanet = _swiss.swe_house_pos(ascmc[2], latitude, eps_true, 'A', planetPos, ref error);
                    Console.WriteLine(houseOfPlanet);
                    diff = --houseOfPlanet * 30 - xx[0] - longtitude;
                }

            }
            return (planetPosition, diff);

        }
        public Image GetChart(DateTime birthDate, double longtitude, double latitude)
        {
            (Dictionary<string, (double X, double Y, string planetName)> planetPosition, double diff) = GetPlanetCoordinate(birthDate, longtitude, latitude);
           // string directoryPath = Directory.GetCurrentDirectory();
          //  string modelPath = Path.Combine(directoryPath, "PSYCTeam.zip");
            Image image = Image.FromFile(@"Images\zodiacChartv2.png");
            var g = Graphics.FromImage(image);
            Image planetImg;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //create House for astro
            g.FillEllipse(Brushes.White, (int)(center.X - houseOutterRadius), center.Y - houseOutterRadius, houseOutterRadius * 2, houseOutterRadius * 2);
            g.DrawEllipse(new Pen(Brushes.Black) { Width = 3, DashCap = System.Drawing.Drawing2D.DashCap.Round }, center.X - houseInnerRadius, center.Y - houseInnerRadius, houseInnerRadius * 2, houseInnerRadius * 2);

            double angleDiff = diff * Math.PI / 180;

            for (int i = 12; i >= 1; i--)
            {
                angleDiff += Math.PI / 6;
                g.DrawLine(
                    new Pen(Brushes.Black) { Width = 2 },
                    new Point()
                    {
                        X = (int)((houseInnerRadius) * Math.Cos(angleDiff)) + center.X,
                        Y = (int)((houseInnerRadius) * Math.Sin(angleDiff)) + center.Y,
                    },
                    new Point()
                    {
                        X = (int)((houseOutterRadius + 5) * Math.Cos(angleDiff)) + center.X,
                        Y = (int)((houseOutterRadius + 5) * Math.Sin(angleDiff)) + center.Y,
                    });

                g.DrawString(i.ToString(), new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, new PointF()
                {
                    X = (float)(((houseInnerRadius + 20) * Math.Cos(angleDiff + Math.PI / 12)) + center.X - 15),
                    Y = (float)(((houseInnerRadius + 20) * Math.Sin(angleDiff + Math.PI / 12)) + center.Y - 15),
                });

            }

            //create Planet for astro
           foreach (var key1 in planetPosition.Keys)
            {
                PointF point = new PointF();
                double angle = (planetPosition[key1].X / 180) * Math.PI;

                point.X = (float)((pointRadius * Math.Cos(angle)) + center.X);
                point.Y = (float)((pointRadius * Math.Sin(angle)) + center.Y);
              
                try
                {
                    planetImg = Image.FromFile(@$"Images\{planetPosition[key1].planetName}.png");
                    planetImg = (Image)(new Bitmap(planetImg, new Size() { Width = 20, Height = 20 }));
                    g.DrawImage(
                            planetImg,
                            new Point()
                            {
                                X = (int)((planetRadius * Math.Cos(angle)) + center.X - 15),
                                Y = (int)((planetRadius * Math.Sin(angle)) + center.Y - 15)
                            }
                        );
                   
                    }
                catch
                {

                }

                g.FillRectangle(Brushes.Crimson, (float)point.X, (float)point.Y, 2, 2);

                point = new PointF()
                {
                    X = (float)(((houseInnerRadius) * Math.Cos(angle)) + center.X),
                    Y = (float)(((houseInnerRadius) * Math.Sin(angle)) + center.Y),
                };
                g.FillRectangle(Brushes.Crimson, (float)point.X, (float)point.Y, 2, 2);

                //create Aspert for astro

                foreach (var key2 in planetPosition.Keys)
                {
                    if (key1 == key2) continue;
                    var planet1 = planetPosition[key1];
                    var planet2 = planetPosition[key2];

                    double degree = Math.Abs(planet1.X - planet2.X);

                    if (degree > 180)
                    {
                        degree = 360 - degree;
                    }
                    double angle1 = (planet1.X / 180) * Math.PI;
                    double angle2 = (planet2.X / 180) * Math.PI;

                    if ((degree >= BlueSpecialScope1.Item1 && degree <= BlueSpecialScope1.Item2)
                        || (degree >= BlueSpecialScope2.Item1 && degree <= BlueSpecialScope2.Item2))
                    {
                        g.DrawLine(BluePen, new PointF()
                        {
                            X = (float)(((houseInnerRadius) * Math.Cos(angle1)) + center.X),
                            Y = (float)(((houseInnerRadius) * Math.Sin(angle1)) + center.Y),
                        }, new PointF()
                        {
                            X = (float)(((houseInnerRadius) * Math.Cos(angle2)) + center.X),
                            Y = (float)(((houseInnerRadius) * Math.Sin(angle2)) + center.Y),
                        });
                    }
                    else if ((degree >= RedSpecialScope1.Item1 && degree <= RedSpecialScope1.Item2) ||
                              (degree >= RedSpecialScope2.Item1 && degree <= RedSpecialScope2.Item2)
                              || (degree >= RedSpecialScope3.Item1 && degree <= RedSpecialScope3.Item2))
                    {
                        g.DrawLine(RedPen, new PointF()
                        {
                            X = (float)(((houseInnerRadius) * Math.Cos(angle1)) + center.X),
                            Y = (float)(((houseInnerRadius) * Math.Sin(angle1)) + center.Y),
                        }, new PointF()
                        {
                            X = (float)(((houseInnerRadius) * Math.Cos(angle2)) + center.X),
                            Y = (float)(((houseInnerRadius) * Math.Sin(angle2)) + center.Y),
                        });
                    }
                    else if ((degree >= GraySpecialScope1.Item1 && degree <= GraySpecialScope1.Item2)
                              || (degree >= GraySpecialScope2.Item1 && degree <= GraySpecialScope2.Item2))
                    {
                        g.DrawLine(GrayPen, new PointF()
                        {
                            X = (float)(((houseInnerRadius) * Math.Cos(angle1)) + center.X),
                            Y = (float)(((houseInnerRadius) * Math.Sin(angle1)) + center.Y),
                        }, new PointF()
                        {
                            X = (float)(((houseInnerRadius) * Math.Cos(angle2)) + center.X),
                            Y = (float)(((houseInnerRadius) * Math.Sin(angle2)) + center.Y),
                        });
                    }
                   // planetImg = Image.FromFile(@$"Images\{planetPosition[key1].planetName}.png");
                 //   planetImg = (Image)(new Bitmap(planetImg, new Size() { Width = 30, Height = 30 }));
                  //  g.DrawImage(planetImg, new Point() { X = center.X - 80, Y = center.Y - 80 });
                  //  Console.WriteLine(planetImg + "  " + planetPosition[key1].planetName + " - " + planetPosition[key1].X);
                 //   g.DrawString(planetPosition[key1].planetName, new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, new PointF()
                  //  {
                 //       X = (int)((planetRadius * Math.Cos(angle)) + center.X - 15),
                //                Y = (int)((planetRadius * Math.Sin(angle)) + center.Y - 15)
              //      });

                }
                /*
                                Console.WriteLine($"planet: {key1} angle: {angle}, point: {point}");*/

                //create info aspert
                //   planetImg = Image.FromFile(@$"Images\{planetPosition[key1].planetName}.png");
                // g.FillEllipse(Brushes.White, center.X - 20, center.Y - 20, 39, 39);
                //  planetImg = (Image)(new Bitmap(planetImg, new Size() { Width = 30, Height = 30 }));
                /* g.DrawString($"planet: {key1}  angle: {angle}, point: {point}", new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, new PointF()
                 {
                     X = (int)((planetRadius * Math.Cos(angle)) + center.X - 50),
                     Y = (int)((planetRadius * Math.Sin(angle)) + center.Y - 50)
                 });*/

                //  Console.WriteLine($"planet: {key1}  angle: {angle*180/Math.PI}");




            }

            //create Planet Earth for astro
            planetImg = Image.FromFile(@$"Images\ea.png");
            g.FillEllipse(Brushes.White, center.X - 20, center.Y - 20, 39, 39);
            planetImg = (Image)(new Bitmap(planetImg, new Size() { Width = 40, Height = 40 }));
            g.DrawImage(planetImg, new Point() { X = center.X - 20, Y = center.Y - 20 });
            g.Flush();

            //chi tiet


            // foreach (var key1 in planetPosition.Keys)


            // Create font and brush.
            Font drawFont = new Font("Arial", 7);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Create rectangle for drawing.
            float x = 535.0F;
            float y = 532.0F;
            float width = 200.0F;
            float height = 50.0F;
           

            // Draw rectangle to screen.
            Pen blackPen = new Pen(Color.Black);
            // g.DrawRectangle(blackPen, x, y, width, height);

            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Near;

            // Draw string to screen.
             foreach (var key1 in planetPosition.Keys) {
                 y = y + 9.0F;
                g.FillEllipse(Brushes.White, x, y, width, height);
                RectangleF drawRect = new RectangleF(x, y, width, height);
                double angle = (planetPosition[key1].X / 180) * Math.PI * 180 / Math.PI;
                double angleR = (planetPosition[key1].X / 180) * Math.PI * 180 / Math.PI;
                if (angle < 0) angle = angle * (-1);
                int nguyen = (int)angle;
                double phut = (angle - nguyen)*60;
                int phutnguyen = (int)phut;
                double giay = Math.Round((phut - phutnguyen),2);
                
                string drawString = $"{key1}:  {nguyen} {phutnguyen}' {giay}''";

                if (angleR < 0)
                {
                     drawString = $"{key1}:  {nguyen} {phutnguyen}' {giay}''R";
                }
                    g.DrawString(drawString, drawFont, drawBrush, drawRect, drawFormat);



                Console.WriteLine($"{key1}:{angle * 180 / Math.PI}");
            }
           




            // Create string to draw.



            //  g.Flush();



            return image;
        }


        public Image GetChartAstro(DateTime birthDate, double longtitude, double latitude)
        {
            (Dictionary<string, (double X, double Y, string planetName)> planetPosition, double diff) = GetPlanetCoordinate(birthDate, longtitude, latitude);
            // string directoryPath = Directory.GetCurrentDirectory();
            //  string modelPath = Path.Combine(directoryPath, "PSYCTeam.zip");
            Image image = Image.FromFile(@"Images\zodiacChartv2.png");
            var g = Graphics.FromImage(image);
            Image planetImg;

           // g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //create House for astro
           // g.FillEllipse(Brushes.White, (int)(center.X - houseOutterRadius), center.Y - houseOutterRadius, houseOutterRadius * 2, houseOutterRadius * 2);
          //  g.DrawEllipse(new Pen(Brushes.Black) { Width = 3, DashCap = System.Drawing.Drawing2D.DashCap.Round }, center.X - houseInnerRadius, center.Y - houseInnerRadius, houseInnerRadius * 2, houseInnerRadius * 2);

            double angleDiff = diff * Math.PI / 180;

            foreach (var key1 in planetPosition.Keys)
            {
                PointF point = new PointF();
                double angle = (planetPosition[key1].X / 180) * Math.PI;

                point.X = (float)((pointRadius * Math.Cos(angle)) + center.X);
                point.Y = (float)((pointRadius * Math.Sin(angle)) + center.Y);

             
                point = new PointF()
                {
                    X = (float)(((houseInnerRadius) * Math.Cos(angle)) + center.X),
                    Y = (float)(((houseInnerRadius) * Math.Sin(angle)) + center.Y),
                };
            
                foreach (var key2 in planetPosition.Keys)
                {
                    if (key1 == key2) continue;
                    var planet1 = planetPosition[key1];
                    var planet2 = planetPosition[key2];

                    double degree = Math.Abs(planet1.X - planet2.X);

                    if (degree > 180)
                    {
                        degree = 360 - degree;
                    }
                    double angle1 = (planet1.X / 180) * Math.PI;
                    double angle2 = (planet2.X / 180) * Math.PI;

    
                  

                }
              




            }
            // Create font and brush.
            Font drawFont = new Font("Arial", 7);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Create rectangle for drawing.
            float x = 535.0F;
            float y = 532.0F;
            float width = 200.0F;
            float height = 50.0F;


            // Draw rectangle to screen.
            Pen blackPen = new Pen(Color.Black);
            // g.DrawRectangle(blackPen, x, y, width, height);

            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Near;

            // Draw string to screen.
            foreach (var key1 in planetPosition.Keys)
            {
                y = y + 9.0F;
                g.FillEllipse(Brushes.White, x, y, width, height);
                RectangleF drawRect = new RectangleF(x, y, width, height);
                double angle = (planetPosition[key1].X / 180) * Math.PI * 180 / Math.PI;
                double angleR = (planetPosition[key1].X / 180) * Math.PI * 180 / Math.PI;
                if (angle < 0) angle = angle * (-1);
                int nguyen = (int)angle;
                double phut = (angle - nguyen) * 60;
                int phutnguyen = (int)phut;
                double giay = Math.Round((phut - phutnguyen), 2);

                string drawString = $"{key1}:  {nguyen} {phutnguyen}' {giay}''";

                if (angleR < 0)
                {
                    drawString = $"{key1}:  {nguyen} {phutnguyen}' {giay}''R";
                }
                g.DrawString(drawString, drawFont, drawBrush, drawRect, drawFormat);



                Console.WriteLine($"{key1}:{angle * 180 / Math.PI}");
            }





            // Create string to draw.



            //  g.Flush();



            return image;
        }

        public string GetChartFile(DateTime birthDate, double longtitude, double latitude)
        {
            Image image = GetChart(birthDate, longtitude, latitude);
            var stream = new MemoryStream();
            string fileName = @$"Images\chart-{Guid.NewGuid()}.png";
            var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            image.Save(file, ImageFormat.Png);
            file.Close();
            return file.Name;
        }

        public string GetChartLinkFirebase(DateTime birthDate, double longtitude, double latitude)
        {

            var fileName = GetChartFile(birthDate, longtitude, latitude);


            var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            string link = UploadChart(file, fileName.Split('\\').Last()).Result;
            file.Close();

            System.IO.File.Delete(fileName);

            return link;
        }

        public async Task<string> UploadChart(Stream image, string name)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            CancellationTokenSource cancel = new CancellationTokenSource();
            var task = new FirebaseStorage(Bucket, new FirebaseStorageOptions()
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true,
            });
            string link = string.Empty;
            try
            {
                link = await task.Child("userchart").Child(name).PutAsync(image);

            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            return link;
        }

        public string GetImageFile(Image base64String)
        {
            Image image = Base64ToImage(base64String);
            var stream = new MemoryStream();
            string fileName = @$"Images\deposit-{Guid.NewGuid()}.png";
            var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            image.Save(file, ImageFormat.Png);
            file.Close();
            return file.Name;
        }
        public string GetImageLinkFirebase(Image base64String)
        {

            var fileName = GetImageFile(base64String);


            var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            string link = UploadImage(file, fileName.Split('\\').Last()).Result;
            file.Close();

            System.IO.File.Delete(fileName);

            return link;
        }
        public Image Base64ToImage(Image base64String)
        {
          //  byte[] imageBytes = Convert.FromBase64String(base64String);
          //  MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
          //  ms.Write(imageBytes, 0, imageBytes.Length);
          //  System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return base64String;
        }

        public async Task<string> UploadImage(Stream image, string name)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            CancellationTokenSource cancel = new CancellationTokenSource();
            var task = new FirebaseStorage(Bucket, new FirebaseStorageOptions()
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true,
            });
            string link = string.Empty;
            try
            {
                link = await task.Child("deposit").Child(name).PutAsync(image);

            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            return link;
        }


    }
}
