using GpmLoginApiV2Sample.Libs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;

using System.Threading;
using System.Web;


namespace GpmLoginApiV2Sample
{
    class GPMLoginAPI_FB
    {
        static string apiUrl = "http://127.0.0.1:19995";
        private string idProfile;

        private string seleniumRemoteDebugAddress;
        private string gpmDriverPath;

        private FileInfo gpmDriverFileInfo;
        private ChromeDriverService service;
        private ChromeOptions options;
        public ChromeDriver driver;

        public GPMLoginAPI_FB(string idProfile)
        {
            this.idProfile = idProfile;
        }

        public void Start()
        {
            GPMLoginAPI api = new GPMLoginAPI(apiUrl);
            JObject startedResult = api.Start(idProfile);
            seleniumRemoteDebugAddress = Convert.ToString(startedResult["selenium_remote_debug_address"]);
            gpmDriverPath = Convert.ToString(startedResult["selenium_driver_location"]);

            // Init selenium
            gpmDriverFileInfo = new FileInfo(gpmDriverPath);
            service = ChromeDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
            options = new ChromeOptions();

            //options.BinaryLocation = browserLocation;
            options.DebuggerAddress = seleniumRemoteDebugAddress;

            //options.AddAdditionalOption("useAutomationExtension", false);
            //options.AddExcludedArgument("enable-automation");
            options.AddArgument("--disable-blink-features");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            driver = new ChromeDriver(service, options);

        }
        public void LoginTDS(string userName, string passWord)
        {
            driver.Navigate().GoToUrl("https://traodoisub.com/");
            Thread.Sleep(5000);
            try
            {
                var txtUserElement = driver.FindElement(By.Id("username"));
                txtUserElement.SendKeys(userName);
                var txtPassElement = driver.FindElement(By.Id("password"));
                txtPassElement.SendKeys(passWord);
                Thread.Sleep(5000);
                txtPassElement.SendKeys(Keys.Enter);
            }
            catch { }
        }

        public List<IdTask> getIdNV(string result)
        {
            List<IdTask> idTasks = new List<IdTask>();
            //driver.Navigate().GoToUrl("https://traodoisub.com/api/?fields=" + type + "&access_token=" + token);
            //Thread.Sleep(5000);
            //var txtBody = driver.FindElement(By.XPath("/html/body/pre"));
            
            JArray jsonArray = JArray.Parse(result);

            try
            {
                var readerJS = JsonConvert.SerializeObject(jsonArray);
                var list = JsonConvert.DeserializeObject<List<IdTask>>(readerJS);
                foreach (var id in list)
                {
                    idTasks.Add(id);
                }
            }
            catch { return null; }
            
            return idTasks;
        }
        public string getJsonAPI(string type, string token)
        {
            string result=null;
            goToUrl("https://traodoisub.com/api/?fields=" + type + "&access_token=" + token);
            Thread.Sleep(7000);
            var txtBody = driver.FindElement(By.XPath("/html/body/pre"));
            result = txtBody.Text;          
            return result;
        }
        public void scollWebpageRandom()
        {
            Console.WriteLine("Cuộn chuột lướt page ...");
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            Random rnd = new Random();
            int YPosition = 0;
            for (int i=0; i < 3; i++)
            {
                YPosition = rnd.Next(300, 900);
                js.ExecuteScript("window.scrollBy(0," + YPosition + ");");
                Thread.Sleep(2000);
                js.ExecuteScript("window.scrollBy(0," + (YPosition-100) + ");");
                Thread.Sleep(2000);
                js.ExecuteScript("window.scrollBy(0," + (YPosition + 100) + ");");
                Thread.Sleep(2000);
                js.ExecuteScript("window.scrollBy(0," + (YPosition + 200) + ");");
                Thread.Sleep(5000);

            }
        }

        public void scollSlowWebpage()
        {
            IWebElement txtBody = driver.FindElement(By.TagName("body"));
            txtBody.SendKeys(Keys.PageDown);
            Thread.Sleep(20000);
            txtBody.SendKeys(Keys.PageDown);
            Thread.Sleep(20000);
            txtBody.SendKeys(Keys.PageDown);
            Thread.Sleep(20000);
            txtBody.SendKeys(Keys.PageDown);
            Thread.Sleep(20000);
            //IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            //js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");        
        }

        public void goToUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public void closeProfile()
        {
            Thread.Sleep(10000);
            Console.WriteLine("CLOSE BROWSER");
            driver.Close();
            driver.Quit();
            driver.Dispose();
        }

        public void openNewTab()
        {
            IWebElement element = driver.FindElement(By.TagName("body"));
            Thread.Sleep(5000);
            element.SendKeys(OpenQA.Selenium.Keys.Control + "t");
        }
        public void getCoin(string type, string id_job, string tokenTDS)
        {
            //openNewTab();
            goToUrl("https://traodoisub.com/api/coin/?type=" + type + "&id=" + id_job + "&access_token=" + tokenTDS);
            Thread.Sleep(7000);
            var txtBody = driver.FindElement(By.XPath("/html/body/pre"));
            Console.WriteLine("Nhận coin: \n");
            Console.WriteLine(txtBody.Text);
        }

    }

}
