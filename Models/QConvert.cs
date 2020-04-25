using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
namespace massage.Models
{
    public static class QConvert
    {
        // PSchedule conversions
        public static Dictionary<string, Dictionary<string, bool>> ScheduleFromQuery(List<PSchedule> PSs) // this function takes a PSchedule query from the database and converts it to an easily readable/parseable dictionary of dictionaries for the front end to work with
        {
            Dictionary<string, Dictionary<string, bool>> formattedSchedule = new Dictionary<string, Dictionary<string, bool>>();
            foreach (PSchedule ps in PSs)
            {
                Dictionary<string, bool> thisPDict = new Dictionary<string, bool>();
                for (int h=6; h<=18; h++)
                {
                    string firstHourDec = "";
                    string secondHourDec = "";
                    int firstT = 0;
                    int secondT = 0;
                    if (h <= 11)
                    {
                        firstHourDec = "am";
                        secondHourDec = "am";
                        firstT = h;
                        secondT = h+1;
                    }
                    if (h == 11)
                    {
                        secondHourDec = "pm";
                    }
                    if (h >= 12)
                    {
                        firstHourDec = "pm";
                        secondHourDec = "pm";
                        firstT = h-12;
                        secondT = h-11;
                    }
                    if (h == 12)
                    {
                        firstT = h;
                    }
                    int t = h;
                    if (t > 12)
                    {
                        t -= 12;
                    }
                    bool thisVal = (bool)ps.GetType().GetProperty("t" + h).GetValue(ps);
                    thisPDict.Add($"{firstT}{firstHourDec} - {secondT}{secondHourDec}", thisVal);
                }
                formattedSchedule.Add(ps.DayOfWeek, thisPDict);
            }
            return formattedSchedule;
        }
        public static List<PSchedule> ScheduleToQuery(Dictionary<string, Dictionary<string, bool>> frontEndPS, int practID)
        {
            List<PSchedule> queryReadyList = new List<PSchedule>();
            foreach (KeyValuePair<string, Dictionary<string, bool>> outerKVP in frontEndPS)
            {
                PSchedule thisPS = new PSchedule();
                thisPS.DayOfWeek = outerKVP.Key;
                thisPS.PractitionerId = practID;
                thisPS.Approved = false;
                foreach (KeyValuePair<string, bool> innerKVP in outerKVP.Value)
                {
                    string unParsedHour = innerKVP.Key.Substring(0,4);
                    string parsedHour = "";
                    int numHour = Int32.Parse(unParsedHour[0].ToString());
                    if (unParsedHour[0] == '1' && unParsedHour[1] != 'p')
                    {
                        parsedHour = unParsedHour.Substring(0,2);
                    }
                    else if (unParsedHour[1] == 'p') // like 3pm, numhour is 3, needs to add 12 so it's 15
                    {
                        parsedHour = (numHour + 12).ToString();
                    }
                    else { // am, like 9am, numhour is 9 so just set it to parsedHour
                        parsedHour = numHour.ToString();
                    }
                    thisPS.GetType().GetProperty("t" + parsedHour).SetValue(thisPS, innerKVP.Value);
                }
                queryReadyList.Add(thisPS);
            }
            return queryReadyList;
        }
        // Json conversions
        public static string TimeslotsToEvents(List<Timeslot> TSs)
        {
            List<object> eventResults = new List<object>();
            foreach (Timeslot ts in TSs)
            {
                long myStart = (long)(ts.Date.AddHours(ts.Hour) - (new DateTime(1970, 1, 1))).TotalMilliseconds;
                var oneEvent = new {
                    id = ts.TimeslotId,
                    title = $"{6 - ts.Reservations.Count} Available",
                    start = myStart,
                    end = (myStart + 3600000), // start + 1 hour in milliseconds.
                    backgroundColor = "#00a65a",
                    borderColor     = "#00a65a", 
                    textColor       = "#ffffff",
                };
                eventResults.Add(oneEvent);
            }
            var jsonEvents = new {
                success = 1,
                result = eventResults
            };
            return JsonConvert.SerializeObject(jsonEvents);
        }
        public static string ReservationsToEvents(List<Reservation> Rs)
        {
            List<object> eventResults = new List<object>();
            foreach (Reservation r in Rs)
            {
                long myStart = (long)(r.Timeslot.Date.AddHours(r.Timeslot.Hour) - (new DateTime(1970, 1, 1))).TotalMilliseconds;
                var oneEvent = new {
                    id = r.ReservationId,
                    title = $"Customer: {r.Customer.FirstName} {r.Customer.LastName[0]}, Service: {r.Service.Name}, Practitioner: {r.Practitioner.FirstName} {r.Practitioner.LastName[0]}",
                    start = myStart,
                    end = (myStart + 3600000), // start + 1 hour in milliseconds.
                    backgroundColor = "#00a65a",
                    borderColor     = "#00a65a", 
                    textColor       = "#ffffff",
                };
                eventResults.Add(oneEvent);
            }
            var jsonEvents = new {
                success = 1,
                result = eventResults
            };
            return JsonConvert.SerializeObject(jsonEvents);
        }
        public static string FilteredEvents(string eventsJson, ProjectContext db)
        {
            JsonFilterObject events = JsonConvert.DeserializeObject<JsonFilterObject>(eventsJson);
            List<Timeslot> parsedTSList = Query.AllFutureTimeslots(db);
            int cID;
            int pID;
            int sID;
            if (events.CustomerId.Length != 0){
                cID = Int32.Parse(events.CustomerId);
                pID = Int32.Parse(events.PractitionerId);
                sID = Int32.Parse(events.ServiceId);
            }
            else {
                cID = 0;
                pID = 0;
                sID = 0;
            }
            if (cID != 0)
            {
                parsedTSList = QueryFilter.ByCustomer(cID, parsedTSList, db);
            }
            if (pID != 0)
            {
                parsedTSList = QueryFilter.ByPractitioner(pID, parsedTSList, db);
            }
            if (sID != 0)
            {
                parsedTSList = QueryFilter.ByService(sID, parsedTSList, db);
            }

            return TimeslotsToEvents(parsedTSList);
        }
    }
}