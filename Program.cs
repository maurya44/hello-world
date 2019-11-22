using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json; //added JSON.NET with Nuget
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace GeoCoding_Art
{
    public class Location
    {
        [Index(0)]
        public string Latitude { get; set; }
        [Index(1)]
        public string Longitude { get; set; }
    }
    public class address
    {
        [Index(0)]
        public string Address { get; set; }
    }
    class Program
    {
        const string apiKey = "AIzaSyDjr3bekgSndnw3v3kMeEBQyYUr5VJl0p8"; //paste your API KEY HERE 
        static string baseUrlGC = "https://maps.googleapis.com/maps/api/geocode/json?address="; // part1 of the URL for GeoCoding
        static string baseUrlRGC = "https://maps.googleapis.com/maps/api/geocode/json?latlng="; // part1 of the URL for Reverse GeoCoding
        static string plusUrl = "&key=" + apiKey + "&sensor=false"; // part2 of the URL

        static public int DisplayMenu() // I add a menu for selecting between 1 - GeoCoding / 2 - Reverse Geocoding / 3 - Exit
        {
            Console.WriteLine("GEOCODING TUTORIAL (Select and hit Enter):");
            Console.WriteLine();
            Console.WriteLine("1. GeoCoding"); // 1 for GeoCoding
            Console.WriteLine("2. Reverse GeoCoding"); // 2 for Reverse Geocoding
            Console.WriteLine("3. Exit"); // 3 Exit
            Console.WriteLine();
            var result = Console.ReadLine(); //waiting for an integer input for the menu; value can between 1-3
            return Convert.ToInt32(result); //converting result to an integer for the menu
        }

        static void Main(string[] args)
        {
            int menuInput = 0;
            StringBuilder header = new StringBuilder();
            string OutputCSVPath = @"D:\output1.csv";
            StringBuilder addressfromlat = new StringBuilder();
            header.AppendLine("Latitude" + "," + "Longitude" + "," + "Address");

            string filepath = @"D:\address.csv"; ;
            IEnumerable<address> locationinfo = null;
            if (!File.Exists(OutputCSVPath))
            {
                File.WriteAllText(OutputCSVPath, header.ToString());
            }
            else
            {
                File.AppendAllText(OutputCSVPath, header.ToString());
            }

            Console.WriteLine("===== REVERSE GEOCODING =====");
            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader))
            {

                csv.Read();


                //csv.Configuration.Delimiter = ",";
                csv.Configuration.MissingFieldFound = null;


                csv.Configuration.HasHeaderRecord = true;
                if (csv != null)
                {
                    locationinfo = csv.GetRecords<address>();
                }
                //foreach (ocation loc in locationinfo)
                //{

                //    //Console.WriteLine("-------------------------------------");
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    string address = ReverseGeoCoding(loc.Latitude, loc.Longitude);
                //    // Console.WriteLine("RESULT: " + address);
                //    Console.ForegroundColor = ConsoleColor.Gray;
                //    addressfromlat.AppendLine(loc.Latitude+","+loc.Longitude+","+address);
                //}
                foreach (address loc in locationinfo)
                {

                    //Console.WriteLine("-------------------------------------");
                    //Console.ForegroundColor = ConsoleColor.Red;
                    string address = GeoCoding(loc.Address);
                    // Console.WriteLine("RESULT: " + address);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                    addressfromlat.AppendLine(address + "," + loc.Address);
                }
                //do // do-while statement for the menu, it loops until the input is 3 (Exit) 
                //{
                //    Console.ForegroundColor = ConsoleColor.Green; // changing color for the console to green 
                //    menuInput=DisplayMenu(); //getting the result of the input for the menu
                //    Console.ForegroundColor = ConsoleColor.Gray; // changing to default color

                //    switch (menuInput.ToString()) //switch statement for checking if input is 1 or 2
                //    {
                //        case "1":    //if the input for menu is 1, then call the GeoCoding function
                //            Console.WriteLine("===== GEOCODING =====");
                //            Console.WriteLine("Enter an address for GeoCoding Result: ");
                //            string inputAddress = Console.ReadLine();
                //            Console.WriteLine("-------------------------------------");
                //            Console.ForegroundColor = ConsoleColor.Red;
                //            Console.WriteLine("RESULT: " + GeoCoding(inputAddress));
                //            Console.ForegroundColor = ConsoleColor.Gray;
                //        break;
                //        case "2":    //if the input for menu is 2, then call the ReverseGeoCoding function
                //            Console.WriteLine("===== REVERSE GEOCODING =====");
                //            Console.WriteLine("Enter a Latitude: ");
                //            string latitude = Console.ReadLine();
                //            Console.WriteLine("Enter a Longitude: ");
                //            string longitude = Console.ReadLine();
                //            Console.WriteLine("-------------------------------------");
                //            Console.ForegroundColor = ConsoleColor.Red;
                //            string address=ReverseGeoCoding(latitude, longitude);
                //            Console.WriteLine("RESULT: " + address);
                //            Console.ForegroundColor = ConsoleColor.Gray;
                //            addressfromlat.AppendLine(address);

                //            break;
                //    }
                //} while (menuInput != 3);

                if (!File.Exists(OutputCSVPath))
                {
                    File.WriteAllText(OutputCSVPath, addressfromlat.ToString());
                }

                File.AppendAllText(OutputCSVPath, addressfromlat.ToString());

            }
        }

        static string GeoCoding(string address)
        {
            try
            {
                var json = new WebClient().DownloadString(baseUrlGC + address.Replace(" ", "+")
                    + plusUrl);//concatenate URL with the input address and downloads the requested resource
                GoogleGeoCodeResponse jsonResult = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(json); //deserializing the result to GoogleGeoCodeResponse

                string status = jsonResult.status; // get status 

                string geoLocation = String.Empty;

                if (status == "OK") //check if status is OK
                {
                    for (int i = 0; i < jsonResult.results.Length; i++) //loop throught the result for lat/lng
                    {
                        geoLocation += jsonResult.results[i].geometry.location.lat +
                        ", " + jsonResult.results[i].geometry.location.lng;
                        break;//append the result addresses to every new line
                    }
                    return geoLocation; //return result
                }
                else
                {
                    return status; //return status / error if not OK
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            }


        public static string ReverseGeoCoding(string latitude, string longitude)
        {
            try {
                var json = new WebClient().DownloadString(baseUrlRGC + latitude.Replace(" ", "") + ","
                    + longitude.Replace(" ", "") + plusUrl);//concatenate URL with the input lat/lng and downloads the requested resource
                GoogleGeoCodeResponse jsonResult = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(json); //deserializing the result to GoogleGeoCodeResponse

                string status = jsonResult.status; // get status

                string geoLocation = String.Empty;

                if (status == "OK") //check if status is OK
                {
                    for (int i = 0; i < jsonResult.results.Length; i++) //loop throught the result for addresses
                    {
                        geoLocation +=   jsonResult.results[i].formatted_address + Environment.NewLine;
                        break;//append the result addresses to every new line
                    }
                    return geoLocation; //return result
                }
                else
                {
                    return status; //return status / error if not OK
                }
            }
            catch(Exception ex)
                {
                return ex.Message;
            }
            }
        }
    }

