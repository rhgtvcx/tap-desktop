﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirportModel;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.Model.AirlinerModel;

namespace TheAirline.Model.GeneralModel.Helpers
{
    //the helpers class for the AI
    public class AIHelpers
    {
        private static Random rnd = new Random();
        //updates a cpu airline
        public static void UpdateCPUAirline(Airline airline)
        {
            int newRouteInterval = 0;
            switch (airline.Mentality)
            {
                case Airline.AirlineMentality.Aggressive:
                    newRouteInterval = 10000;
                    break;
                case Airline.AirlineMentality.Moderate:
                    newRouteInterval = 100000;
                    break;
                case Airline.AirlineMentality.Safe:
                    newRouteInterval = 1000000;
                    break;
            }

            Boolean newRoute = rnd.Next(newRouteInterval)/1000 == 0;

            //creates a new route for the airline
            if (newRoute)
            {
                List<Airport> homeAirports = airline.Airports.FindAll(a => a.getAirportFacility(airline, AirportFacility.FacilityType.Service).TypeLevel > 0);
                homeAirports.AddRange(airline.Airports.FindAll(a => a.Hubs.Count(h => h.Airline == airline) > 0)); //hubs

                Airport airport = homeAirports.Find(a => a.Terminals.getFreeGates(airline) > 0);
                
                if (airport == null)
                {
                    airport = homeAirports.Find(a => a.Terminals.getFreeGates() > 0);
                    if (airport != null)
                        airport.Terminals.rentGate(airline);
                    else
                    {
                        airport = GetServiceAirport(airline);
                        if (airport != null)
                            airport.Terminals.rentGate(airline);
                    }
                    
                }

                if (airport != null)
                {
                    Airport destination = GetDestinationAirport(airline, airport);

            
                    if (destination != null)
                    {

                        KeyValuePair<Airliner,Boolean>? airliner = GetAirlinerForRoute(airline, airport, destination);

                        if (airliner.HasValue)
                        {
                            if (destination.Terminals.getFreeGates(airline) == 0) destination.Terminals.rentGate(airline);

                            if (!airline.Airports.Contains(destination)) airline.addAirport(destination);

                            double price = PassengerHelpers.GetPassengerPrice(airport, destination);

                            Guid id = Guid.NewGuid();

                            Route route = new Route(id.ToString(), airport, destination, price, airline.getNextFlightCode(), airline.getNextFlightCode());

                            foreach (AirlinerClass.ClassType type in Enum.GetValues(typeof(AirlinerClass.ClassType)))
                            {
                                route.getRouteAirlinerClass(type).CabinCrew = 2;
                                route.getRouteAirlinerClass(type).DrinksFacility = RouteFacilities.GetFacilities(RouteFacility.FacilityType.Drinks)[rnd.Next(RouteFacilities.GetFacilities(RouteFacility.FacilityType.Drinks).Count)];// RouteFacilities.GetBasicFacility(RouteFacility.FacilityType.Drinks);
                                route.getRouteAirlinerClass(type).FoodFacility = RouteFacilities.GetFacilities(RouteFacility.FacilityType.Food)[rnd.Next(RouteFacilities.GetFacilities(RouteFacility.FacilityType.Food).Count)];//RouteFacilities.GetBasicFacility(RouteFacility.FacilityType.Food);
                            }

                            airline.addRoute(route);

                            airport.Terminals.getEmptyGate(airline).Route = route;
                            destination.Terminals.getEmptyGate(airline).Route = route;

                            if (Countries.GetCountryFromTailNumber(airliner.Value.Key.TailNumber).Name != airline.Profile.Country.Name)
                                airliner.Value.Key.TailNumber = airline.Profile.Country.TailNumbers.getNextTailNumber();

                            FleetAirliner fAirliner = new FleetAirliner(FleetAirliner.PurchasedType.Bought, airline, airliner.Value.Key, airliner.Value.Key.TailNumber, airport);

                            RouteAirliner rAirliner = new RouteAirliner(fAirliner, route);

                            if (airliner.Value.Value) //loan
                            {
                                double amount = airliner.Value.Key.getPrice() - airline.Money + 20000000;

                                Loan loan = new Loan(GameObject.GetInstance().GameTime, amount, 120, GeneralHelpers.GetAirlineLoanRate(airline));

                                double payment = loan.getMonthlyPayment();

                                airline.addLoan(loan);
                                airline.addInvoice(new Invoice(loan.Date, Invoice.InvoiceType.Loans, loan.Amount));
                            }
                            else
                                airline.addInvoice(new Invoice(GameObject.GetInstance().GameTime, Invoice.InvoiceType.Purchases, -airliner.Value.Key.getPrice()));

                            fAirliner.RouteAirliner = rAirliner;

                            airline.Fleet.Add(fAirliner);

                            rAirliner.Status = RouteAirliner.AirlinerStatus.To_route_start;
                        }
                    }
                }



            }
        }
        //returns the destination for an airline with a start airport
        public static Airport GetDestinationAirport(Airline airline, Airport airport)
        {
            double maxDistance = (from a in Airliners.GetAirlinersForSale()
                                  select a.Type.Range).Max();

            double minDistance = (from a in Airports.GetAirports().FindAll(a => a != airport) select MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates)).Min();
  
     
            List<Airport> airports = new List<Airport>();
            List<Route> routes = airline.Routes.FindAll(r => r.Destination1 == airport || r.Destination2 == airport);
                  
            switch (airline.MarketFocus)
            {
                case Airline.AirlineMarket.Global:
                    airports = Airports.GetAirports().FindAll(a => MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) < maxDistance && MathHelpers.GetDistance(a.Profile.Coordinates,airport.Profile.Coordinates)>100);
                    break;
                case Airline.AirlineMarket.Local:
                    airports = Airports.GetAirports().FindAll(a => MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) < Math.Max(minDistance, 1000) && MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) > 50);
                    break;
                case Airline.AirlineMarket.Regional:
                    airports = Airports.GetAirports(airport.Profile.Country.Region).FindAll(a => MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) < maxDistance && MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) > 100);
                    break;
            }
           
            Airport destination = null;
            int counter = 0;

            if (airports.Count == 0)
                airports = (from a in Airports.GetAirports().FindAll(a => MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) < 2000 && MathHelpers.GetDistance(a.Profile.Coordinates, airport.Profile.Coordinates) > 50) orderby a.Profile.Size descending select a).ToList();

            while (destination == null && counter < airports.Count)
            {
                destination = airports[counter];

                if ((routes.Find(r => r.Destination1 == destination || r.Destination2 == destination) != null) || (destination.Terminals.getFreeGates()==0 && destination.Terminals.getFreeGates(airline)==0) || (destination == airport)) destination = null;
                counter++;
                
            }
            return destination;
        }
        //returns the best fit for an airliner for sale for a route true for loan
        public static KeyValuePair<Airliner,Boolean>? GetAirlinerForRoute(Airline airline, Airport destination1, Airport destination2)
        {
            double maxLoanTotal = 100000000;
            double distance = MathHelpers.GetDistance(destination1.Profile.Coordinates, destination2.Profile.Coordinates);

            AirlinerType.TypeRange rangeType = GeneralHelpers.ConvertDistanceToRangeType(distance);

            List<Airliner> airliners = Airliners.GetAirlinersForSale().FindAll(a => a.getPrice() < airline.Money - 1000000 && a.getAge() < 10 && distance < a.Type.Range && rangeType == a.Type.RangeType);

            if (airliners.Count > 0)
                return new KeyValuePair<Airliner,Boolean>((from a in airliners orderby a.Type.Range select a).First(),false);
            else
            {
                if (airline.Mentality == Airline.AirlineMentality.Aggressive)
                {
                    double airlineLoanTotal = airline.Loans.Sum(l => l.PaymentLeft);

                    if (airlineLoanTotal < maxLoanTotal)
                    {
                        List<Airliner> loanAirliners = Airliners.GetAirlinersForSale().FindAll(a => a.getPrice() < airline.Money + maxLoanTotal - airlineLoanTotal && a.getAge() < 10 && distance < a.Type.Range && rangeType == a.Type.RangeType);

                        if (loanAirliners.Count > 0)
                            return new KeyValuePair<Airliner,Boolean>((from a in loanAirliners orderby a.Price select a).First(),true);
                        else
                            return null;
                    }
                    else
                        return null;
                    
                }
                else
                    return null;
            }
                
   
        }
        //finds an airport and creates a basic service facility for an airline
        private static Airport GetServiceAirport(Airline airline)
        {
          
            AirportFacility facility = AirportFacilities.GetFacilities(AirportFacility.FacilityType.Service).Find(f=>f.TypeLevel==1);

            var airports = from a in airline.Airports.FindAll(aa => aa.Terminals.getFreeGates() > 0) orderby a.Profile.Size descending select a;

            if (airports.Count() > 0)
            {
                Airport airport = airports.First();

                airport.setAirportFacility(airline, facility);

                return airport;
            }

            return null;

        }
    }
}