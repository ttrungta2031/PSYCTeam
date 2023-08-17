using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Tables;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultSurveysController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private readonly IDrawnatalchart _drawnatal;
        private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

        private static string Bucket = "psycteamv1.appspot.com";
        private static string AuthEmail = "tuanninh655@gmail.com";
        private static string AuthPassword = "bivsan1";
        public ResultSurveysController(PsychologicalCouselingContext context, IDrawnatalchart drawnatal )
        {
            _context = context;
            _drawnatal = drawnatal;
        }

        // GET: api/ResultSurveys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultSurvey>>> GetResultSurveys()
        {
            return await _context.ResultSurveys.ToListAsync();
        }

        // GET: api/ResultSurveys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultSurvey>> GetResultSurvey(int id)
        {
            var resultSurvey = await _context.ResultSurveys.FindAsync(id);

            if (resultSurvey == null)
            {
                return NotFound();
            }

            return resultSurvey;
        }




        public class SurveyResultBody
        {
            public int CustomerId { get; set; }
            public List<int> OptionId { get; set; }
        }

        // PUT: api/ResultSurveys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet("getresultsurveyandbirthchart")]
        public async Task<IActionResult> GetResultSurveyAndBirthChartAsync(int customerid)
        {
            //    string format = "dd/MM/yyyy";
            var custom = await _context.Customers.FindAsync(customerid);

            if (custom == null || customerid<0)
            {
                return NotFound();
            }
            //   var resultidmax = _context.ResponseResults.Where(a => a.CustomerId == customerid).Max(b => b.Id);
            //  var result = _context.ResponseResults.Where(a => a.CustomerId == customerid && a.Id == resultidmax).FirstOrDefault();
            int flagmaxresult = 0;
            var resultidmax = _context.ResponseResults.Where(a => a.CustomerId == customerid).Any();
            if (resultidmax == true)
            {
                flagmaxresult = _context.ResponseResults.Where(a => a.CustomerId == customerid).Max(i => i.Id);
            }


            var result = _context.ResponseResults.Where(a => a.CustomerId == customerid && a.Id == flagmaxresult).FirstOrDefault();


            if (result == null)
            {
                return StatusCode(404, new { StatusCode = 404, Message = "Customer hasn't survey already!" });
            }
            var birthchart = _context.Customers.Where(a => a.Id == customerid).FirstOrDefault();

            return Ok(new { StatusCode = 200, Message = "Load successful", resultofsurvey = result.DetailResult , discchart = result.Description, birthchart = birthchart.Birthchart});
        }






        // POST: api/ResultSurveys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("submitsurveybylist")]
        public async Task<ActionResult<ResultSurvey>> SubmitSurveyByList([FromBody]SurveyResultBody option)
        {
            try
            {

                // var all = _context.Specializations.Where(us => us.ConsultantId.Equals(id));
                var result = new ResultSurvey();
                int numberquestion = 1;
                for (int i = 0; i < option.OptionId.Count; i++)
                {
                    int optionidnew = _context.ResultSurveys.Max(it => it.Id);
                    var optionid = _context.OptionQuestions.Where(a => a.Id == option.OptionId[i]).FirstOrDefault();
                    var questionnew = _context.Questions.Where(a => a.Id == optionid.QuestionId).FirstOrDefault();
                    optionidnew = optionidnew + 2;
                    result.Id = optionidnew;
                    result.CustomerId = option.CustomerId;
                    result.QuestionId = questionnew.Id;
                    result.OptionQuestionId = option.OptionId[i];
                    _context.ResultSurveys.Add(result);                
                    numberquestion++;
                    await _context.SaveChangesAsync();
                }



                /* foreach (var question in questionid)
                 {

                     {
                         result.CustomerId = customerid;
                         result.QuestionId = question;

                     }
                     _context.ResultSurveys.Add(result);
                     await _context.SaveChangesAsync();
                 }


                 foreach (var option in optionid)
                 {
                     result.OptionQuestionId = option;

                     _context.ResultSurveys.Update(result);
                     await _context.SaveChangesAsync();
                 }*/









                double D = 0;
                double I = 0;
                double S = 0;
                double C = 0;
                int total = 1;
                var resultnew = (from s in _context.ResultSurveys
                              where s.CustomerId == option.CustomerId
                              select new
                              {
                                  Id = s.Id,
                                  Optionid = s.OptionQuestionId,
                                  Typeoption = s.OptionQuestion.Type

                              }).ToList();
                foreach (var item in resultnew)
                {
                    if (item.Typeoption == "D") D = D + 1;
                    else if (item.Typeoption == "I") I = I + 1;
                    else if (item.Typeoption == "S") S = S + 1;
                    else if (item.Typeoption == "C") C = C + 1;
                }

                total = resultnew.Count();
                string ketquakhaosat = "";
                if (D > I && D > S && D > C) ketquakhaosat = "Người thuộc nhóm (Sự thống trị) sẽ có tính cách nhanh nhẹn, hoạt bát, mạnh mẽ, tự tin, chủ động, tập trung, năng động, hướng tới kết quả công việc";
                if (I > D && I > S && I > C) ketquakhaosat = "Người thuộc nhóm (Ảnh Hưởng) sẽ có tính cách nhiệt tình, cởi mở, vui vẻ, hòa nhã, lạc quan, thích cái mới, sáng tạo, hướng tới con người";
                if (S > D && S > I && S > C) ketquakhaosat = "Người thuộc nhóm (Kiên Định) sẽ có tính cách điềm đạm, từ tốn, ổn định, chín chắn, kiên định, lắng nghe có kế hoạch, đáng tin cậy, tận tâm, trách nhiệm, quan tâm tới con người";
                if (C > D && C > I && C > S) ketquakhaosat = "Người thuộc nhóm (Tuân Thủ) sẽ có tính cách chính xác, bình tĩnh, cầu toàn, cẩn trọng, trật tự, đúng đắn, tập trung, công bằng, rõ ràng, thận trọng, tư duy Logic, hướng tới kỹ thuật";
                D = Math.Round(D * 100 / total,2);
                S = Math.Round(S * 100 / total,2);
                I = Math.Round(I * 100 / total,2);
                C = 100 - D - S -I;


                string resultcustom = "Tổng quan: D: " + Math.Round(D, 2) + "%, I: " + Math.Round(I, 2) + "%, S: " + Math.Round(S, 2) + "%, C: " + Math.Round(C, 2) + "%. " + ketquakhaosat;
                var customer = _context.Customers.Where(a => a.Id == option.CustomerId).FirstOrDefault();

              /*  Aspose.Words.License lic = new Aspose.Words.License();
                try
                {
                    lic.SetLicense("Aspose.Words.lic");
                    Console.WriteLine("License set successfully.");
                }
                catch (Exception e)
                {
                    // We do not ship any license with this example, visit the Aspose site to obtain either a temporary or permanent license. 
                    Console.WriteLine("\nThere was an error setting the license: " + e.Message);
                }*/
            //    lic.SetLicense("Aspose.Words.lic");
                var doc = new Document();
                var builder = new DocumentBuilder(doc);
           //     builder.InsertField(" MERGEFIELD somot ");
               
             //   builder.InsertField(" MERGEFIELD soba ");
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                Shape shape = builder.InsertChart(ChartType.Pie3D, 350, 180);
                Chart chart = shape.Chart;
                chart.Title.Text = "Báo Cáo Kết Quả Bài Trắc Nghiệm DISC";

                chart.Series.Clear();

                chart.Series.Add("Tongquan",
                    new string[] { "D - Sự thống trị", "I - Ảnh Hưởng", "S - Kiên Định", "C - Tuân Thủ" },
                    new double[] { D, I, S, C });

                builder.Writeln();
                ParagraphFormat paragraphFormat1 = builder.ParagraphFormat;
                paragraphFormat1.Alignment = ParagraphAlignment.Center;
                paragraphFormat1.LeftIndent = 50;
                paragraphFormat1.RightIndent = 50;
                paragraphFormat1.SpaceAfter = 25;
                builder.Bold = true;
                builder.Font.Size = 11;
                builder.Font.Color = Color.MediumPurple;
                builder.Writeln("KẾT QUẢ: " + customer.Fullname + " là " + ketquakhaosat);
                builder.ParagraphFormat.ClearFormatting();
                builder.Writeln();



                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.Bold = true;
                builder.Font.Color = Color.WhiteSmoke;
                builder.StartTable();

                builder.CellFormat.ClearFormatting();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.CellFormat.Width = 50;
                //  builder.CellFormat.RightPadding = 5;
                builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                builder.CellFormat.Shading.BackgroundPatternColor = Color.DarkSlateGray;
                builder.CellFormat.WrapText = false;
                builder.CellFormat.FitText = true;


                builder.RowFormat.ClearFormatting();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.RowFormat.HeightRule = HeightRule.Auto;
                //    builder.RowFormat.
                builder.RowFormat.Height = 35;
                builder.RowFormat.Borders.LineStyle = LineStyle.Engrave3D;
                builder.RowFormat.Borders.Color = Color.Orange;

                builder.InsertCell();
                builder.Write("D");
                builder.InsertCell();
                builder.Write("I");
                // builder.EndRow();
                builder.InsertCell();
                builder.Write("S");
                builder.InsertCell();
                builder.Write("C");
                builder.EndRow();
                builder.InsertCell();
                builder.Write(Math.Round(D, 2).ToString() + "%");
                builder.InsertCell();
                builder.Write(Math.Round(I, 2).ToString() + "%");
                // builder.EndRow();
                builder.InsertCell();
                builder.Write(Math.Round(S, 2).ToString() + "%");
                builder.InsertCell();
                builder.Write(Math.Round(C, 2).ToString() + "%");
                builder.EndTable();



                builder.InsertField(" MERGEFIELD soba ");
                builder.MoveToMergeField("soba");
                builder.ParagraphFormat.ClearFormatting();
                builder.Writeln();
               /* ParagraphFormat paragraphFormat1 = builder.ParagraphFormat;
                paragraphFormat1.Alignment = ParagraphAlignment.Center;
                paragraphFormat1.LeftIndent = 50;
                paragraphFormat1.RightIndent = 50;
                paragraphFormat1.SpaceAfter = 25;
                builder.Bold = true;
                builder.Font.Size = 11;
                builder.Font.Color = Color.MediumPurple;
                builder.Writeln("KẾT QUẢ: "+ customer.Fullname + " là "+ ketquakhaosat);*/



                ParagraphFormat paragraphFormat = builder.ParagraphFormat;
                paragraphFormat.Alignment = ParagraphAlignment.Center;
                paragraphFormat.LeftIndent = 50;
                paragraphFormat.RightIndent = 50;
                paragraphFormat.SpaceAfter = 25;
                builder.Bold = true;
                builder.Font.Size = 10;
                builder.Font.Color = Color.DarkGoldenrod;




            
                builder.Writeln("- D: Chính xác, dứt khoát và có định hướng. Những người này có xu hướng độc lập và chú trọng nhiều tới kết quả. Họ là những người có ý chí mạnh mẽ, thích thử thách, hành động và mong muốn đạt được kết quả ngay lập tức;");
                builder.Writeln("- I: Lạc quan và hướng ngoại. Những người này có xu hướng giao tiếp xã hội cao. Họ thích tham gia vào các hội nhóm để được chia sẻ suy nghĩ và tiếp thêm năng lượng cho những người khác;");
                builder.Writeln("- S: Đồng cảm và hợp tác. Những người này có xu hướng trở thành đồng đội tốt, vì họ thích làm việc sau hậu trường để hỗ trợ và giúp đỡ những người khác. Họ cũng là những người biết lắng nghe và luôn tìm cách hạn chế xung đột;");
                builder.Writeln("- C: Quan tâm, Thận trọng & Chính xác. Những người này thường tập trung vào chi tiết và chất lượng. Họ luôn lên kế hoạch trước khi hành động, liên tục kiểm tra độ chính xác và luôn đặt ra câu hỏi “làm thế nào”/“tại sao”.");

              //  builder.InsertField(" MERGEFIELD sohai ");
             //   builder.MoveToMergeField("sohai");
              
                doc.Save("Output.pdf");

                var extractedPage = doc.ExtractPages(0, 1);
                extractedPage.Save($"Output_{1}.jpg");
                string fileName = @$"Images\pdf-{Guid.NewGuid()}.png";
                extractedPage.Save(fileName);
                var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);



                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));

                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);


                var cancel = new CancellationTokenSource();

                var upload = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child("images").Child("pdf").PutAsync(file, cancel.Token);

                // await upload;

                string link = await upload;

                //  return Ok(new { StatusCode = 200, Message = "Upload FIle success", data = link });


                var response = new ResponseResult();
                {
                    response.DetailResult = resultcustom;
                    response.Description = link;
                    response.CustomerId = option.CustomerId;
                }
                _context.ResponseResults.Add(response);
                await _context.SaveChangesAsync();




                return Ok(new { StatusCode = 201, Message = "Submit Survey Successfull" , linkresult = link});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


  /*      [HttpPost("submitsurveybylist")]
        public async Task<ActionResult<ResultSurvey>> SubmitSurveyByList(SurveyResultBody option)
        {
            try
            {

                // var all = _context.Specializations.Where(us => us.ConsultantId.Equals(id));
                var result = new ResultSurvey();
                int numberquestion = 1;
                for (int i = 0; i < optionid.Count; i++)
                {
                    int optionidnew = _context.ResultSurveys.Max(it => it.Id);
                    var option = _context.OptionQuestions.Where(a => a.Id == optionid[i]).FirstOrDefault();
                    var questionnew = _context.Questions.Where(a => a.Id == option.QuestionId).FirstOrDefault();
                    optionidnew = optionidnew + 2;
                    result.Id = optionidnew;
                    result.CustomerId = customerid;
                    result.QuestionId = questionnew.Id;
                    result.OptionQuestionId = optionid[i];
                    _context.ResultSurveys.Add(result);
                    numberquestion++;
                    await _context.SaveChangesAsync();
                }



                *//* foreach (var question in questionid)
                 {

                     {
                         result.CustomerId = customerid;
                         result.QuestionId = question;

                     }
                     _context.ResultSurveys.Add(result);
                     await _context.SaveChangesAsync();
                 }


                 foreach (var option in optionid)
                 {
                     result.OptionQuestionId = option;

                     _context.ResultSurveys.Update(result);
                     await _context.SaveChangesAsync();
                 }*//*









                double D = 0;
                double I = 0;
                double S = 0;
                double C = 0;
                int total = 1;
                var resultnew = (from s in _context.ResultSurveys
                                 where s.CustomerId == customerid
                                 select new
                                 {
                                     Id = s.Id,
                                     Optionid = s.OptionQuestionId,
                                     Typeoption = s.OptionQuestion.Type

                                 }).ToList();
                foreach (var item in resultnew)
                {
                    if (item.Typeoption == "D") D = D + 1;
                    else if (item.Typeoption == "I") I = I + 1;
                    else if (item.Typeoption == "S") S = S + 1;
                    else if (item.Typeoption == "C") C = C + 1;
                }

                total = resultnew.Count();
                string ketquakhaosat = "";
                if (D > I && D > S && D > C) ketquakhaosat = "Người thuộc nhóm này sẽ có tính cách nhanh nhẹn, hoạt bát, mạnh mẽ, tự tin, chủ động, tập trung, năng động, hướng tới kết quả công việc";
                if (I > D && I > S && I > C) ketquakhaosat = "Người thuộc nhóm này sẽ có tính cách nhiệt tình, cởi mở, vui vẻ, hòa nhã, lạc quan, thích cái mới, sáng tạo, hướng tới con người";
                if (S > D && S > I && S > C) ketquakhaosat = "Người thuộc nhóm này sẽ có tính cách điềm đạm, từ tốn, ổn định, chín chắn, kiên định, lắng nghe có kế hoạch, đáng tin cậy, tận tâm, trách nhiệm, quan tâm tới con người";
                if (C > D && C > I && C > S) ketquakhaosat = "Người thuộc nhóm này sẽ có tính cách chính xác, bình tĩnh, cầu toàn, cẩn trọng, trật tự, đúng đắn, tập trung, công bằng, rõ ràng, thận trọng, tư duy Logic, hướng tới kỹ thuật";
                D = Math.Round(D * 100 / total, 2);
                S = Math.Round(S * 100 / total, 2);
                I = Math.Round(I * 100 / total, 2);
                C = 100 - D - S - I;


                string resultcustom = "Tổng quan: D: " + Math.Round(D, 2) + "%, I: " + Math.Round(I, 2) + "%, S: " + Math.Round(S, 2) + "%, C: " + Math.Round(C, 2) + "%. " + ketquakhaosat;
                var customer = _context.Customers.Where(a => a.Id == customerid).FirstOrDefault();

                *//*  Aspose.Words.License lic = new Aspose.Words.License();
                  try
                  {
                      lic.SetLicense("Aspose.Words.lic");
                      Console.WriteLine("License set successfully.");
                  }
                  catch (Exception e)
                  {
                      // We do not ship any license with this example, visit the Aspose site to obtain either a temporary or permanent license. 
                      Console.WriteLine("\nThere was an error setting the license: " + e.Message);
                  }*//*
                //    lic.SetLicense("Aspose.Words.lic");
                var doc = new Document();
                var builder = new DocumentBuilder(doc);
                //     builder.InsertField(" MERGEFIELD somot ");

                //   builder.InsertField(" MERGEFIELD soba ");
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                Shape shape = builder.InsertChart(ChartType.Pie3D, 350, 180);
                Chart chart = shape.Chart;
                chart.Title.Text = "Báo Cáo Kết Quả Bài Trắc Nghiệm DISC";

                chart.Series.Clear();

                chart.Series.Add("Tongquan",
                    new string[] { "D - Sự thống trị", "I - Ảnh Hưởng", "S - Kiên Định", "C - Tuân Thủ" },
                    new double[] { D, I, S, C });






                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.Writeln();
                builder.Writeln();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.Bold = true;
                builder.Font.Color = Color.WhiteSmoke;
                builder.StartTable();

                builder.CellFormat.ClearFormatting();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.CellFormat.Width = 50;
                //  builder.CellFormat.RightPadding = 5;
                builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                builder.CellFormat.Shading.BackgroundPatternColor = Color.DarkSlateGray;
                builder.CellFormat.WrapText = false;
                builder.CellFormat.FitText = true;


                builder.RowFormat.ClearFormatting();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.RowFormat.HeightRule = HeightRule.Auto;
                //    builder.RowFormat.
                builder.RowFormat.Height = 35;
                builder.RowFormat.Borders.LineStyle = LineStyle.Engrave3D;
                builder.RowFormat.Borders.Color = Color.Orange;

                builder.InsertCell();
                builder.Write("D");
                builder.InsertCell();
                builder.Write("I");
                // builder.EndRow();
                builder.InsertCell();
                builder.Write("S");
                builder.InsertCell();
                builder.Write("C");
                builder.EndRow();
                builder.InsertCell();
                builder.Write(Math.Round(D, 2).ToString() + "%");
                builder.InsertCell();
                builder.Write(Math.Round(I, 2).ToString() + "%");
                // builder.EndRow();
                builder.InsertCell();
                builder.Write(Math.Round(S, 2).ToString() + "%");
                builder.InsertCell();
                builder.Write(Math.Round(C, 2).ToString() + "%");
                builder.EndTable();



                builder.InsertField(" MERGEFIELD soba ");
                builder.MoveToMergeField("soba");
                builder.ParagraphFormat.ClearFormatting();

                ParagraphFormat paragraphFormat = builder.ParagraphFormat;
                paragraphFormat.Alignment = ParagraphAlignment.Center;
                paragraphFormat.LeftIndent = 50;
                paragraphFormat.RightIndent = 50;
                paragraphFormat.SpaceAfter = 25;
                builder.Bold = true;
                builder.Font.Size = 10;
                builder.Font.Color = Color.DarkGoldenrod;




                builder.Writeln();
                builder.Writeln("DISC là từ viết tắt của 4 đặc điểm tính cách gồm Dominance (Thống trị), Influence (Ảnh hưởng), Steadiness (Kiên định) và Conscientiousness (Tuân Thủ). Trong đó:");
                builder.Writeln("- D: Chính xác, dứt khoát và có định hướng. Những người này có xu hướng độc lập và chú trọng nhiều tới kết quả. Họ là những người có ý chí mạnh mẽ, thích thử thách, hành động và mong muốn đạt được kết quả ngay lập tức;");
                builder.Writeln("- I: Lạc quan và hướng ngoại. Những người này có xu hướng giao tiếp xã hội cao. Họ thích tham gia vào các hội nhóm để được chia sẻ suy nghĩ và tiếp thêm năng lượng cho những người khác;");
                builder.Writeln("- S: Đồng cảm và hợp tác. Những người này có xu hướng trở thành đồng đội tốt, vì họ thích làm việc sau hậu trường để hỗ trợ và giúp đỡ những người khác. Họ cũng là những người biết lắng nghe và luôn tìm cách hạn chế xung đột;");
                builder.Writeln("- C: Quan tâm, Thận trọng & Chính xác. Những người này thường tập trung vào chi tiết và chất lượng. Họ luôn lên kế hoạch trước khi hành động, liên tục kiểm tra độ chính xác và luôn đặt ra câu hỏi “làm thế nào”/“tại sao”.");

                //  builder.InsertField(" MERGEFIELD sohai ");
                //   builder.MoveToMergeField("sohai");

                doc.Save("Output.pdf");

                var extractedPage = doc.ExtractPages(0, 1);
                extractedPage.Save($"Output_{1}.jpg");
                string fileName = @$"Images\pdf-{Guid.NewGuid()}.png";
                extractedPage.Save(fileName);
                var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);



                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));

                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);


                var cancel = new CancellationTokenSource();

                var upload = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child("images").Child("pdf").PutAsync(file, cancel.Token);

                // await upload;

                string link = await upload;

                //  return Ok(new { StatusCode = 200, Message = "Upload FIle success", data = link });


                var response = new ResponseResult();
                {
                    response.DetailResult = resultcustom;
                    response.Description = link;
                    response.CustomerId = customerid;
                }
                _context.ResponseResults.Add(response);
                await _context.SaveChangesAsync();




                return Ok(new { StatusCode = 201, Message = "Submit Survey Successfull", linkresult = link });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
*/

        // DELETE: api/ResultSurveys/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteResultSurvey(int id)
        {
            var resultSurvey = await _context.ResultSurveys.FindAsync(id);
            if (resultSurvey == null)
            {
                return NotFound();
            }

            _context.ResultSurveys.Remove(resultSurvey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultSurveyExists(int id)
        {
            return _context.ResultSurveys.Any(e => e.Id == id);
        }
    }
}
