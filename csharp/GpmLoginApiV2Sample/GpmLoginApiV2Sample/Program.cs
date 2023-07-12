using GpmLoginApiV2Sample.Libs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Web;

namespace GpmLoginApiV2Sample
{
    internal class Program
    {
        static string apiUrl = "http://127.0.0.1:19995";
        static void Main(string[] args)
        {
            // khởi tạo GPM Login
            GPMLoginAPI_FB gpm = new GPMLoginAPI_FB("67517958-cc8a-4cfa-8c32-c634a29f3a1d");

            // mở profile
            gpm.Start();
            Thread.Sleep(2000);

            //dùng utf8
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //token TDS
            string tokenTDS = "TDSQfiUjclZXZzJiOiIXZ2V2ciwiIzUTM2ITM2kTOwkDMwATMiojIyV2c1Jye";

            //lấy ds nv - likepage - số vòng lặp là i
            int i = 2;
            string resultJS = gpm.getJsonAPI("page", tokenTDS);
            Console.WriteLine("Lấy danh sách nhiệm vụ: ");
            Console.WriteLine(resultJS);
            List<IdTask> listID_NVLikePage = gpm.getIdNV(resultJS);
            if (listID_NVLikePage != null)
            {
                foreach(IdTask idTask in listID_NVLikePage)
                {
                    likePage(gpm, idTask.id);
                    Thread.Sleep(10000);
                    gpm.openNewTab();
                    getCoin(gpm, "PAGE", idTask.id, tokenTDS);
                    Console.WriteLine("Dừng chờ 80s lướt new feed");
                    gpm.goToUrl("https://www.facebook.com/");
                    Thread.Sleep(5000);
                    gpm.scollSlowWebpage();
                }
            }
            else
            {
                Console.WriteLine("Không có nhiệm vụ");
            }
        }

        // Auto like page
        static void likePage(GPMLoginAPI_FB gpm, string id)
        {
            gpm.goToUrl("https://www.facebook.com/" + id);
            Thread.Sleep(10000);
            try
            {
                var btnLike = gpm.driver.FindElement(By.XPath("(//div[@aria-label=\"Thích\"])[1]"));
                btnLike.Click();
                Console.WriteLine("Like thành công");
            }
            catch { Console.WriteLine("Like không thành công"); }
            gpm.scollWebpageRandom();
        }

        //lấy xu
        static void getCoin(GPMLoginAPI_FB gpm, string type, string id_job, string tokenTDS)
        {
            gpm.openNewTab();
            gpm.goToUrl("https://traodoisub.com/api/coin/?type=" + type + "&id=" + id_job + "&access_token=" + tokenTDS);
            Thread.Sleep(7000);
            var txtBody = gpm.driver.FindElement(By.XPath("/html/body/pre"));
            Console.WriteLine("Nhận coin: \n");
            Console.WriteLine(txtBody.Text);
        }

        //dừng chờ
        
    }

   
}
