﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheAirlineV2.Model.AirportModel;
using System.Xml;
using TheAirlineV2.Model.AirlineModel;
using TheAirlineV2.Model.AirlinerModel;
using TheAirlineV2.Model.GeneralModel.StatisticsModel;
using TheAirlineV2.Model.AirlinerModel.RouteModel;
using System.IO;
using System.IO.Compression;

namespace TheAirlineV2.Model.GeneralModel.Helpers
{
    public class LoadSaveHelpers
    {
        private static string dataPath = AppDomain.CurrentDomain.BaseDirectory + "data\\saves";
        //loads a game
        public static void LoadGame(string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(dataPath + "\\" + name + ".xml");
            XmlElement root = doc.DocumentElement;

           
            Airliners.Clear();

            XmlNodeList airlinersList = root.SelectNodes("//airliners/airliner");

            foreach (XmlElement airlinerNode in airlinersList)
            {
                AirlinerType type = AirlinerTypes.GetType(airlinerNode.Attributes["type"].Value);
                //string type = airlinerNode.Attributes["type"].Value;
                string tailnumber = airlinerNode.Attributes["tailnumber"].Value;
                string last_service = airlinerNode.Attributes["last_service"].Value;
                DateTime built = Convert.ToDateTime(airlinerNode.Attributes["built"].Value);
                double flown = Convert.ToDouble(airlinerNode.Attributes["flown"].Value);

                Airliner airliner = new Airliner(type, tailnumber, built);
                airliner.Flown = flown;
                airliner.clearAirlinerClasses();

                XmlNodeList airlinerClassList = airlinerNode.SelectNodes("classes/class");

                foreach (XmlElement airlinerClassNode in airlinerClassList)
                {
                    AirlinerClass.ClassType airlinerClassType =  (AirlinerClass.ClassType)Enum.Parse(typeof(AirlinerClass.ClassType), airlinerClassNode.Attributes["type"].Value);
                    int airlinerClassSeating = Convert.ToInt16(airlinerClassNode.Attributes["seating"].Value);

                    AirlinerClass aClass = new AirlinerClass(airliner,airlinerClassType,airlinerClassSeating);

                    airliner.addAirlinerClass(aClass);
                }
                Airliners.AddAirliner(airliner);
               
            }

            List<Airline> airlines = Airlines.GetAirlines();
            Airlines.Clear();

            
            XmlNodeList airlinesList = root.SelectNodes("//airlines/airline");

            /*
             * loanNode.SetAttribute("date", loan.Date.ToShortDateString());
                    loanNode.SetAttribute("rate", loan.Rate.ToString());
                    loanNode.SetAttribute("amount", loan.Amount.ToString());
                    loanNode.SetAttribute("lenght", loan.Lenght.ToString());
                    loanNode.SetAttribute("payment", loan.PaymentLeft.ToString());*/
            foreach (XmlElement airlineNode in airlinesList)
            {
                string airlineName = airlineNode.Attributes["name"].Value;
                string airlineCEO = airlineNode.Attributes["CEO"].Value;
                double money = Convert.ToDouble(airlineNode.Attributes["money"].Value);
                int reputation = Convert.ToInt16(airlineNode.Attributes["reputation"].Value);

                Airline airline = airlines.Find(delegate(Airline a) { return a.Profile.IATACode == airlineName; });
                airline.Fleet.Clear();
                airline.Airports.Clear();
                airline.Routes.Clear();
              
                airline.Money = money;
                airline.Profile.CEO = airlineCEO;
                airline.Reputation = reputation;

                XmlNodeList airlineFacilitiesList = airlineNode.SelectNodes("facilities/facilty");
                foreach (XmlElement airlineFacilityNode in airlineFacilitiesList)
                {
                    string airlineFacility = airlineFacilityNode.Attributes["name"].Value;
                    
                    airline.addFacility(AirlineFacilities.GetFacility(airlineFacility));
                }

                XmlNodeList airlineLoanList = airlineNode.SelectNodes("loans/loan");
                foreach (XmlElement airlineLoanNode in airlineLoanList)
                {
                    DateTime date = Convert.ToDateTime(airlineLoanNode.Attributes["date"].Value);
                    double rate = Convert.ToDouble(airlineLoanNode.Attributes["rate"].Value);
                    double amount = Convert.ToDouble(airlineLoanNode.Attributes["amount"].Value);
                    int lenght = Convert.ToInt16(airlineLoanNode.Attributes["lenght"].Value);
                    double payment = Convert.ToDouble(airlineLoanNode.Attributes["payment"]);

                    Loan loan = new Loan(date, amount, lenght, rate);
                    loan.PaymentLeft = payment;

                    airline.addLoan(loan);
                }

                XmlNodeList airlineStatList = airlineNode.SelectNodes("stats/stat");
                foreach (XmlElement airlineStatNode in airlineStatList)
                {
                    string airlineStatType = airlineStatNode.Attributes["type"].Value;
                    int value = Convert.ToInt16(airlineStatNode.Attributes["value"].Value);

                    airline.Statistics.setStatisticsValue(StatisticsTypes.GetStatisticsType(airlineStatType), value);
                }

                XmlNodeList airlineInvoiceList = airlineNode.SelectNodes("invoices/invoice");
    
                foreach (XmlElement airlineInvoiceNode in airlineInvoiceList)
                {
                    Invoice.InvoiceType type = (Invoice.InvoiceType)Enum.Parse(typeof(Invoice.InvoiceType), airlineInvoiceNode.Attributes["type"].Value);
                    DateTime invoiceDate = Convert.ToDateTime(airlineInvoiceNode.Attributes["date"].Value);
                    double invoiceAmount = Convert.ToDouble(airlineInvoiceNode.Attributes["amount"].Value);

                    airline.addInvoice(new Invoice(invoiceDate, type, invoiceAmount));
                }

                XmlNodeList airlineFleetList = airlineNode.SelectNodes("fleet/airliner");

                foreach (XmlElement airlineAirlinerNode in airlineFleetList)
                {
                    Airliner airliner = Airliners.GetAirliner(airlineAirlinerNode.Attributes["airliner"].Value);
                    string fAirlinerName = airlineAirlinerNode.Attributes["name"].Value;
                    Airport homebase = Airports.GetAirport(airlineAirlinerNode.Attributes["homebase"].Value);
                    FleetAirliner.PurchasedType purchasedtype = (FleetAirliner.PurchasedType)Enum.Parse(typeof(FleetAirliner.PurchasedType), airlineAirlinerNode.Attributes["purchased"].Value);
                   
                    FleetAirliner fAirliner = new FleetAirliner(purchasedtype,airline,airliner,fAirlinerName,homebase);
                              
                    XmlNodeList airlinerStatList = airlineAirlinerNode.SelectNodes("stats/stat");

                    foreach (XmlElement airlinerStatNode in airlinerStatList)
                    {
                        string statType = airlinerStatNode.Attributes["type"].Value;
                        int statValue = Convert.ToInt16(airlinerStatNode.Attributes["value"].Value);
                        fAirliner.Statistics.setStatisticsValue(StatisticsTypes.GetStatisticsType(statType),statValue);
                    }
                              
                    airline.addAirliner(fAirliner);

                }
                XmlNodeList routeList = airlineNode.SelectNodes("routes/route");

                foreach (XmlElement routeNode in routeList)
                {
                    string id = routeNode.Attributes["id"].Value;
                    Airport dest1 = Airports.GetAirport(routeNode.Attributes["destination1"].Value);
                    Airport dest2 = Airports.GetAirport(routeNode.Attributes["destination2"].Value);
                    
                    Route route = new Route(id, dest1, dest2,0,"","");
                    route.Classes.Clear();

                 
                    XmlNodeList routeClassList = routeNode.SelectNodes("routeclasses/routeclass");

                    foreach (XmlElement routeClassNode in routeClassList)
                    {
                        AirlinerClass.ClassType airlinerClassType = (AirlinerClass.ClassType)Enum.Parse(typeof(AirlinerClass.ClassType), routeClassNode.Attributes["type"].Value);
                        double fareprice = Convert.ToDouble(routeClassNode.Attributes["fareprice"].Value);
                        int cabincrew = Convert.ToInt16(routeClassNode.Attributes["cabincrew"].Value);
                        RouteFacility drinks = RouteFacilities.GetFacilities(RouteFacility.FacilityType.Drinks).Find(delegate(RouteFacility facility) { return facility.Name == routeClassNode.Attributes["drinks"].Value; }); 
                        RouteFacility food = RouteFacilities.GetFacilities(RouteFacility.FacilityType.Food).Find(delegate(RouteFacility facility) { return facility.Name == routeClassNode.Attributes["food"].Value; }); 

                        RouteAirlinerClass rClass = new RouteAirlinerClass(airlinerClassType, fareprice);
                        rClass.CabinCrew = cabincrew;
                        rClass.DrinksFacility = drinks;
                        rClass.FoodFacility = food;

                        route.addRouteAirlinerClass(rClass);        

                    }

                    RouteTimeTable timeTable = new RouteTimeTable(route);

                    XmlNodeList timetableList = routeNode.SelectNodes("timetable/timetableentry");

                    foreach (XmlElement entryNode in timetableList)
                    {
                        Airport entryDest = Airports.GetAirport(entryNode.Attributes["destination"].Value);
                        string flightCode = entryNode.Attributes["flightcode"].Value;
                        DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), entryNode.Attributes["day"].Value);
                        TimeSpan time = TimeSpan.Parse(entryNode.Attributes["time"].Value);

                        timeTable.addEntry(new RouteTimeTableEntry(timeTable, day, time, new RouteEntryDestination(entryDest, flightCode)));
                    }
                    route.TimeTable = timeTable;

                    XmlElement routeAirlinerNode = (XmlElement)routeNode.SelectSingleNode("routeairliner");

                    if (routeAirlinerNode != null)
                    {
                        FleetAirliner fAirliner = airline.Fleet.Find(delegate(FleetAirliner fa) { return fa.Name == routeAirlinerNode.Attributes["airliner"].Value; });
                        
                        RouteAirliner.AirlinerStatus status = (RouteAirliner.AirlinerStatus)Enum.Parse(typeof(RouteAirliner.AirlinerStatus), routeAirlinerNode.Attributes["status"].Value);
                        Coordinate latitude = Coordinate.Parse(routeAirlinerNode.Attributes["latitude"].Value);
                        Coordinate longitude = Coordinate.Parse(routeAirlinerNode.Attributes["longitude"].Value);

                        RouteAirliner rAirliner = new RouteAirliner(fAirliner, route);
                        rAirliner.CurrentPosition = new Coordinates(latitude, longitude);
                        rAirliner.Status = status;
                        
                        XmlElement flightNode = (XmlElement)routeAirlinerNode.SelectSingleNode("flight");
                        if (flightNode != null)
                        {

                            string destination = flightNode.Attributes["destination"].Value;
                            DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), flightNode.Attributes["day"].Value);
                            TimeSpan time = TimeSpan.Parse(flightNode.Attributes["time"].Value);

                            RouteTimeTableEntry rtte = timeTable.Entries.Find(delegate(RouteTimeTableEntry e) { return e.Destination.FlightCode == destination && e.Day == day && e.Time == time; }); 

                            Flight currentFlight = new Flight(rtte);
                            currentFlight.Classes.Clear();
                         
                            XmlNodeList flightClassList = flightNode.SelectNodes("flightclasses/flightclass");

                            foreach (XmlElement flightClassNode in flightClassList)
                            {
                                AirlinerClass.ClassType airlinerClassType = (AirlinerClass.ClassType)Enum.Parse(typeof(AirlinerClass.ClassType), flightClassNode.Attributes["type"].Value);
                                int passengers = Convert.ToInt16(flightClassNode.Attributes["passengers"].Value);

                                currentFlight.Classes.Add(new FlightAirlinerClass(route.getRouteAirlinerClass(airlinerClassType),passengers));
                            }
                            rAirliner.CurrentFlight = currentFlight;
                        }
                    }
          
                    airline.Routes.Add(route);
                }


                Airlines.AddAirline(airline);

    
            }
            XmlNodeList airportsList = root.SelectNodes("//airports/airport");


            foreach (XmlElement airportNode in airportsList)
            {
                Airport airport = Airports.GetAirport(airportNode.Attributes["iata"].Value);
                AirportProfile.AirportSize airportSize = (AirportProfile.AirportSize)Enum.Parse(typeof(AirportProfile.AirportSize), airportNode.Attributes["size"].Value);
                airport.Profile.Size = airportSize;


                XmlNodeList airportStatList = airportNode.SelectNodes("stats/stat");

                foreach (XmlElement airportStatNode in airportStatList)
                {
                    Airline airline = Airlines.GetAirline(airportStatNode.Attributes["airline"].Value);
                    string statType = airportStatNode.Attributes["type"].Value; 
                    int statValue = Convert.ToInt32(airportStatNode.Attributes["value"].Value);
                    airport.Statistics.setStatisticsValue(airline,StatisticsTypes.GetStatisticsType(statType), statValue);
                }

                XmlNodeList airportFacilitiesList = airportNode.SelectNodes("facilities/facility");

                foreach (XmlElement airportFacilityNode in airportFacilitiesList)
                {
                    Airline airline = Airlines.GetAirline(airportFacilityNode.Attributes["airline"].Value);
                    AirportFacility airportFacility = AirportFacilities.GetFacility(airportFacilityNode.Attributes["name"].Value);

                    airport.setAirportFacility(airline, airportFacility);
                }
                airport.Gates.clear();

                XmlNodeList airportGatesList = airportNode.SelectNodes("gates/gate");

                foreach (XmlElement airportGateNode in airportGatesList)
                {
                    Gate gate = new Gate(airport);
                    if (airportGateNode.Attributes["airline"].Value.Length > 0)
                    {
                        Airline airline = Airlines.GetAirline(airportGateNode.Attributes["airline"].Value);
                        gate.Airline = airline;

                        if (airportGateNode.Attributes["route"].Value.Length > 0)
                        {
                            string routeId = airportGateNode.Attributes["route"].Value;
                            airline.Routes.Find(delegate(Route r) { return r.Id == airportGateNode.Attributes["route"].Value; });
                        }
                    }
                    airport.Gates.addGate(gate);
                }
 
            }

            XmlElement gameSettingsNode = (XmlElement)root.SelectSingleNode("//gamesettings");

            GameObject.GetInstance().Name = gameSettingsNode.Attributes["name"].Value;

            Airline humanAirline = Airlines.GetAirline(gameSettingsNode.Attributes["human"].Value);
            GameObject.GetInstance().HumanAirline = humanAirline;

            DateTime gameTime = Convert.ToDateTime(gameSettingsNode.Attributes["time"].Value);
            GameObject.GetInstance().GameTime = gameTime;

            double fuelPrice = Convert.ToDouble(gameSettingsNode.Attributes["fuelprice"].Value);
            GameObject.GetInstance().FuelPrice = fuelPrice;

            GameTimeZone timezone = TimeZones.GetTimeZones().Find(delegate(GameTimeZone gtz) { return gtz.UTCOffset == TimeSpan.Parse(gameSettingsNode.Attributes["timezone"].Value);  });
            GameObject.GetInstance().TimeZone = timezone;

            GameObject.GetInstance().NewsBox.MailsOnLandings = Convert.ToBoolean(gameSettingsNode.Attributes["mailonlandings"].Value);

            XmlNodeList newsList = gameSettingsNode.SelectNodes("news/new");
            GameObject.GetInstance().NewsBox.clear();

            foreach (XmlElement newsNode in newsList)
            {
                DateTime newsDate = Convert.ToDateTime(newsNode.Attributes["date"].Value);
                News.NewsType newsType = (News.NewsType)Enum.Parse(typeof(News.NewsType), newsNode.Attributes["type"].Value);
                string newsSubject = newsNode.Attributes["subject"].Value;
                string newsBody = newsNode.Attributes["body"].Value;
                Boolean newsIsRead = Convert.ToBoolean(newsNode.Attributes["isread"].Value);

                News news = new News(newsType, newsDate, newsSubject, newsBody);
                news.IsRead = newsIsRead;


                GameObject.GetInstance().NewsBox.addNews(news);
                                
            }
            foreach (Airline airline in Airlines.GetAirlines())
            {
                foreach (Route route in airline.Routes)
                {
                    route.Destination1.Gates.getEmptyGate(airline).Route = route;
                    route.Destination2.Gates.getEmptyGate(airline).Route = route;

                }
            }

            
        }
        //append a file to the list of saved files
        public static void AppendSavedFile(string name, string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(dataPath + "\\saves.xml");
            XmlElement root = doc.DocumentElement;

            XmlNode node = root.SelectSingleNode("//saves");

            XmlElement e = doc.CreateElement("save");
            e.SetAttribute("name", name);
            e.SetAttribute("file", file);

            node.AppendChild(e);

            doc.Save(dataPath + "\\saves.xml");

        }
        //returns the names of the saved games
        public static List<KeyValuePair<string, string>> GetSavedGames()
        {
            List<KeyValuePair<string, string>> saves = new List<KeyValuePair<string, string>>();
            
            XmlDocument doc = new XmlDocument();
            doc.Load(dataPath + "\\saves.xml");
            XmlElement root = doc.DocumentElement;

            XmlNodeList savesList = root.SelectNodes("//saves/save");

            foreach (XmlElement saveNode in savesList)
            {
                string name = saveNode.Attributes["name"].Value;
                string file = saveNode.Attributes["file"].Value;
                saves.Add(new KeyValuePair<string, string>(name, file));
            }
            return saves;


        }
        //saves the game
        public static void SaveGame(string file)
        {
            string path = dataPath + "\\" + file + ".xml";

            XmlDocument xmlDoc = new XmlDocument();

            XmlTextWriter xmlWriter = new XmlTextWriter(path, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("game");
            xmlWriter.Close();
            xmlDoc.Load(path);

            XmlNode root = xmlDoc.DocumentElement;


            XmlElement airlinersNode = xmlDoc.CreateElement("airliners");

            foreach (Airliner airliner in Airliners.GetAirliners())
            {
                XmlElement airlinerNode = xmlDoc.CreateElement("airliner");
                airlinerNode.SetAttribute("type", airliner.Type.Name);
                airlinerNode.SetAttribute("tailnumber", airliner.TailNumber);
                airlinerNode.SetAttribute("last_service", airliner.LastServiceCheck.ToString());
                airlinerNode.SetAttribute("built", airliner.BuiltDate.ToShortDateString());
                airlinerNode.SetAttribute("flown", airliner.Flown.ToString());

                XmlElement airlinerClassesNode = xmlDoc.CreateElement("classes");
                foreach (AirlinerClass aClass in airliner.Classes)
                {
                    XmlElement airlinerClassNode = xmlDoc.CreateElement("class");
                    airlinerClassNode.SetAttribute("type", aClass.Type.ToString());
                    airlinerClassNode.SetAttribute("seating", aClass.SeatingCapacity.ToString());

                    airlinerClassesNode.AppendChild(airlinerClassNode);
                }
                airlinerNode.AppendChild(airlinerClassesNode);

                airlinersNode.AppendChild(airlinerNode);
            }

            root.AppendChild(airlinersNode);


            XmlElement airlinesNode = xmlDoc.CreateElement("airlines");
            foreach (Airline airline in Airlines.GetAirlines())
            {
                XmlElement airlineNode = xmlDoc.CreateElement("airline");
                airlineNode.SetAttribute("name", airline.Profile.IATACode);
                airlineNode.SetAttribute("CEO", airline.Profile.CEO);
                airlineNode.SetAttribute("money", airline.Money.ToString());
                airlineNode.SetAttribute("reputation", airline.Reputation.ToString());
                
                XmlElement airlineFacilitiesNode = xmlDoc.CreateElement("facilities");

                foreach (AirlineFacility facility in airline.Facilities)
                {
                    XmlElement airlineFacilityNode = xmlDoc.CreateElement("facility");
                    airlineFacilitiesNode.SetAttribute("name", facility.Shortname);

                    airlineFacilitiesNode.AppendChild(airlineFacilityNode);
                }
                airlineNode.AppendChild(airlineFacilitiesNode);


                XmlElement loansNode = xmlDoc.CreateElement("loans");
                foreach (Loan loan in airline.Loans)
                {
                    XmlElement loanNode = xmlDoc.CreateElement("loan");
                    loanNode.SetAttribute("date", loan.Date.ToShortDateString());
                    loanNode.SetAttribute("rate", loan.Rate.ToString());
                    loanNode.SetAttribute("amount", loan.Amount.ToString());
                    loanNode.SetAttribute("lenght", loan.Lenght.ToString());
                    loanNode.SetAttribute("payment", loan.PaymentLeft.ToString());

                    loansNode.AppendChild(loanNode);
                }
                airlineNode.AppendChild(loansNode);


                XmlElement airlineStatsNode = xmlDoc.CreateElement("stats");
                foreach (StatisticsType type in StatisticsTypes.GetStatisticsTypes())
                {
                    XmlElement airlineStatNode = xmlDoc.CreateElement("stat");

                    int value = airline.Statistics.getStatisticsValue(type);
                    airlineStatNode.SetAttribute("type", type.Shortname);
                    airlineStatNode.SetAttribute("value", value.ToString());

                    airlineStatsNode.AppendChild(airlineStatNode);
                }
                airlineNode.AppendChild(airlineStatsNode);

                XmlElement invoicesNode = xmlDoc.CreateElement("invoices");
                foreach (Invoice invoice in airline.getInvoices())
                {
                    XmlElement invoiceNode = xmlDoc.CreateElement("invoice");
                    invoiceNode.SetAttribute("type", invoice.Type.ToString());
                    invoiceNode.SetAttribute("date", invoice.Date.ToShortDateString());
                    invoiceNode.SetAttribute("amount", invoice.Amount.ToString());

                    invoicesNode.AppendChild(invoiceNode);
                }
                airlineNode.AppendChild(invoicesNode);

                XmlElement fleetNode = xmlDoc.CreateElement("fleet");
                foreach (FleetAirliner airliner in airline.Fleet)
                {
                    XmlElement fleetAirlinerNode = xmlDoc.CreateElement("airliner");
                    fleetAirlinerNode.SetAttribute("airliner", airliner.Airliner.TailNumber);
                    fleetAirlinerNode.SetAttribute("name", airliner.Name);
                    fleetAirlinerNode.SetAttribute("homebase", airliner.Homebase.Profile.IATACode);
                    fleetAirlinerNode.SetAttribute("purchased", airliner.Purchased.ToString());

                    XmlElement airlinerStatsNode = xmlDoc.CreateElement("stats");
                    foreach (StatisticsType type in StatisticsTypes.GetStatisticsTypes())
                    {
                        XmlElement airlinerStatNode = xmlDoc.CreateElement("stat");

                        int value = airliner.Statistics.getStatisticsValue(type);
                        airlinerStatNode.SetAttribute("type", type.Shortname);
                        airlinerStatNode.SetAttribute("value", value.ToString());

                        airlinerStatsNode.AppendChild(airlinerStatNode);
                    }

                    fleetAirlinerNode.AppendChild(airlinerStatsNode);

                    fleetNode.AppendChild(fleetAirlinerNode);
                }
                airlineNode.AppendChild(fleetNode);

                XmlElement routesNode = xmlDoc.CreateElement("routes");
                foreach (Route route in airline.Routes)
                {
                    XmlElement routeNode = xmlDoc.CreateElement("route");
                    routeNode.SetAttribute("id", route.Id);
                    routeNode.SetAttribute("destination1", route.Destination1.Profile.IATACode);
                    routeNode.SetAttribute("destination2", route.Destination2.Profile.IATACode);

                    XmlElement routeClassesNode = xmlDoc.CreateElement("routeclasses");

                    foreach (RouteAirlinerClass aClass in route.Classes)
                    {
                        XmlElement routeClassNode = xmlDoc.CreateElement("routeclass");
                        routeClassNode.SetAttribute("type", aClass.Type.ToString());
                        routeClassNode.SetAttribute("fareprice", aClass.FarePrice.ToString());
                        routeClassNode.SetAttribute("cabincrew", aClass.CabinCrew.ToString());
                        routeClassNode.SetAttribute("drinks", aClass.DrinksFacility.Name);
                        routeClassNode.SetAttribute("food", aClass.FoodFacility.Name);

                        routeClassesNode.AppendChild(routeClassNode);
                    }
                    routeNode.AppendChild(routeClassesNode);

                    XmlElement timetableNode = xmlDoc.CreateElement("timetable");

                    foreach (RouteTimeTableEntry entry in route.TimeTable.Entries)
                    {
                        XmlElement ttEntryNode = xmlDoc.CreateElement("timetableentry");
                        ttEntryNode.SetAttribute("destination", entry.Destination.Airport.Profile.IATACode);
                        ttEntryNode.SetAttribute("flightcode", entry.Destination.FlightCode);
                        ttEntryNode.SetAttribute("day", entry.Day.ToString());
                        ttEntryNode.SetAttribute("time", entry.Time.ToString());

                        timetableNode.AppendChild(ttEntryNode);
                    }

                    routeNode.AppendChild(timetableNode);

                    if (route.Airliner != null)
                    {
                        XmlElement routeAirlinerNode = xmlDoc.CreateElement("routeairliner");
              
                        routeAirlinerNode.SetAttribute("airliner", route.Airliner.Airliner.Name);
                        routeAirlinerNode.SetAttribute("status", route.Airliner.Status.ToString());
                        routeAirlinerNode.SetAttribute("latitude", route.Airliner.CurrentPosition.Latitude.ToString());
                        routeAirlinerNode.SetAttribute("longitude", route.Airliner.CurrentPosition.Longitude.ToString());

                         if (route.Airliner.CurrentFlight != null)
                        {
                            XmlElement flightNode = xmlDoc.CreateElement("flight");
                 
                       
                            flightNode.SetAttribute("destination", route.Airliner.CurrentFlight.Entry.Destination.FlightCode);
                            flightNode.SetAttribute("day", route.Airliner.CurrentFlight.Entry.Day.ToString());
                            flightNode.SetAttribute("time", route.Airliner.CurrentFlight.Entry.Time.ToString());

                      
                            XmlElement flightClassesNode = xmlDoc.CreateElement("flightclasses");
                            foreach (FlightAirlinerClass aClass in route.Airliner.CurrentFlight.Classes)
                            {
                                XmlElement flightClassNode = xmlDoc.CreateElement("flightclass");
                                flightClassNode.SetAttribute("type", aClass.AirlinerClass.Type.ToString());
                                flightClassNode.SetAttribute("passengers", aClass.Passengers.ToString());

                                flightClassesNode.AppendChild(flightClassNode);
                            }
                            flightNode.AppendChild(flightClassesNode);

                            routeAirlinerNode.AppendChild(flightNode);

                        }
                      

                        routeNode.AppendChild(routeAirlinerNode);

                    }
               
                    routesNode.AppendChild(routeNode);
                }
                airlineNode.AppendChild(routesNode);



                airlinesNode.AppendChild(airlineNode);
            }
            root.AppendChild(airlinesNode);


            XmlElement airportsNode = xmlDoc.CreateElement("airports");
            foreach (Airport airport in Airports.GetAirports())
            {
                XmlElement airportNode = xmlDoc.CreateElement("airport");
                airportNode.SetAttribute("iata", airport.Profile.IATACode);
                airportNode.SetAttribute("size", airport.Profile.Size.ToString());

                XmlElement airportStatsNode = xmlDoc.CreateElement("stats");
                foreach (Airline airline in Airlines.GetAirlines())
                {
                    foreach (StatisticsType type in StatisticsTypes.GetStatisticsTypes())
                    {
                        XmlElement airportStatNode = xmlDoc.CreateElement("stat");

                        int value = airport.Statistics.getStatisticsValue(airline, type);
                        airportStatNode.SetAttribute("airline", airline.Profile.IATACode);
                        airportStatNode.SetAttribute("type", type.Shortname);
                        airportStatNode.SetAttribute("value", value.ToString());

                        airportStatsNode.AppendChild(airportStatNode);
                    }
                }

                airportNode.AppendChild(airportStatsNode);

                XmlElement airportFacilitiesNode = xmlDoc.CreateElement("facilities");
                foreach (Airline airline in Airlines.GetAirlines())
                {
                    foreach (AirportFacility facility in airport.getAirportFacilities(airline))
                    {
                        XmlElement airportFacilityNode = xmlDoc.CreateElement("facility");
                        airportFacilityNode.SetAttribute("airline", airline.Profile.IATACode);
                        airportFacilityNode.SetAttribute("name", facility.Shortname);

                        airportFacilitiesNode.AppendChild(airportFacilityNode);
                    }
                }
                airportNode.AppendChild(airportFacilitiesNode);

                XmlElement gatesNode = xmlDoc.CreateElement("gates");
                foreach (Gate gate in airport.Gates.getGates())
                {
                    XmlElement gateNode = xmlDoc.CreateElement("gate");
                    gateNode.SetAttribute("airline", gate.Airline == null ? "" : gate.Airline.Profile.IATACode);
                    gateNode.SetAttribute("route", gate.Route == null ? "" : gate.Route.Id);

                    gatesNode.AppendChild(gateNode);
                }
                airportNode.AppendChild(gatesNode);

                airportsNode.AppendChild(airportNode);



            }
            root.AppendChild(airportsNode);
            
            XmlElement gameSettingsNode = xmlDoc.CreateElement("gamesettings");
            gameSettingsNode.SetAttribute("name", GameObject.GetInstance().Name);
            gameSettingsNode.SetAttribute("human", GameObject.GetInstance().HumanAirline.Profile.IATACode);
            gameSettingsNode.SetAttribute("time", GameObject.GetInstance().GameTime.ToString());
            gameSettingsNode.SetAttribute("fuelprice", GameObject.GetInstance().FuelPrice.ToString());
            gameSettingsNode.SetAttribute("timezone", GameObject.GetInstance().TimeZone.UTCOffset.ToString());
            gameSettingsNode.SetAttribute("mailonlandings", GameObject.GetInstance().NewsBox.MailsOnLandings.ToString());
            XmlElement newsNodes = xmlDoc.CreateElement("news");

            foreach (News news in GameObject.GetInstance().NewsBox.getNews())
            {
                XmlElement newsNode = xmlDoc.CreateElement("new");
                newsNode.SetAttribute("date", news.Date.ToString());
                newsNode.SetAttribute("type", news.Type.ToString());
                newsNode.SetAttribute("subject", news.Subject);
                newsNode.SetAttribute("body", news.Body);
                newsNode.SetAttribute("isread", news.IsRead.ToString());

                newsNodes.AppendChild(newsNode);
            }
            gameSettingsNode.AppendChild(newsNodes);

            root.AppendChild(gameSettingsNode);


            xmlDoc.Save(path);

            
        }
    }
}
