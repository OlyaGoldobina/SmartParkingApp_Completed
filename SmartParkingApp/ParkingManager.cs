using Newtonsoft.Json;
using SmartParkingApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartParkingApp {
    public class ParkingManager
    {
        private List<ParkingSession> pastSessions;
        private List<ParkingSession> activeSessions;
        private List<Tariff> tariffTable;

        private List<User> users;

        private int parkingCapacity;
        private int freeLeavePeriod;
        private int nextTicketNumber;

        public string ActivePhoneNumber;

        public ParkingManager() 
        {
            LoadData();
        }

        public ParkingSession EnterParking(string carPlateNumber)
        {
            if(activeSessions.Count >= parkingCapacity || activeSessions.Any(s => s.CarPlateNumber == carPlateNumber))
                return null;

            var session = new ParkingSession
            {
                CarPlateNumber = carPlateNumber,
                EntryDt = DateTime.Now,
                TicketNumber = nextTicketNumber++,
                User = users.FirstOrDefault(u => u.CarPlateNumber == carPlateNumber)
            };
            session.UserId = session.User?.Id;

            activeSessions.Add(session);
            SaveData();
            return session;
        }

        public bool TryLeaveParkingWithTicket(int ticketNumber, out ParkingSession session)
        {
            session = GetSessionByTicketNumber(ticketNumber);

            var currentDt = DateTime.Now;  // Getting the current datetime only once

            var diff = (currentDt - (session.PaymentDt ?? session.EntryDt)).TotalMinutes;
            if(diff <= freeLeavePeriod) {
                session.TotalPayment = 0;
                CompleteSession(session, currentDt);
                return true;
            }
            else {
                session = null;
                return false;
            }
        }  
        
        public decimal GetRemainingCost(int ticketNumber)
        {   
            var currentDt = DateTime.Now;
            var session = GetSessionByTicketNumber(ticketNumber);

            var diff = (currentDt - (session.PaymentDt ?? session.EntryDt)).TotalMinutes;
            return GetCost(diff);
        }

        public void PayForParking(int ticketNumber, decimal amount)
        {
            var session = GetSessionByTicketNumber(ticketNumber);
            session.TotalPayment = (session.TotalPayment ?? 0) + amount;            
            session.PaymentDt = DateTime.Now;
            SaveData();
        }
       
        public bool TryLeaveParkingByCarPlateNumber(string carPlateNumber, out ParkingSession session)
        {
            session = activeSessions.FirstOrDefault(s => s.CarPlateNumber == carPlateNumber);
            if(session == null)
                return false;

            var currentDt = DateTime.Now;

            if(session.PaymentDt != null) {
                if ((currentDt - session.PaymentDt.Value).TotalMinutes <= freeLeavePeriod) {
                    CompleteSession(session, currentDt);
                    return true;
                }
                else {
                    session = null;
                    return false;
                }
            }
            else {
                // No payment, within free leave period -> allow exit
                if((currentDt - session.EntryDt).TotalMinutes <= freeLeavePeriod) {
                    session.TotalPayment = 0;
                    CompleteSession(session, currentDt);
                    return true;
                }
                else {
                    // The session has no connected customer
                    if (session.User == null) {
                        session = null;
                        return false;
                    }
                    else {
                        session.TotalPayment = GetCost((currentDt - session.EntryDt).TotalMinutes - freeLeavePeriod);
                        session.PaymentDt = currentDt;
                        CompleteSession(session, currentDt);
                        return true;
                    }
                }
            }
        }

        public bool AddUser(string name, string phonenumber, string carplatenumber, string password)
        {
            if (users.Any(x => x.Phone == phonenumber))
                return false;
            else
            {
                users.Add(new User {Name = name, Phone = phonenumber, CarPlateNumber = carplatenumber, Password = password, Id = CountUserNumber()});
                SaveUserData();
                return true;
            }
        }

        public bool UserExisting(string phonenumber, string password)
        {
            if (users.Any(x => x.Phone == phonenumber & x.Password == password))
            {
                ActivePhoneNumber = phonenumber;
                return true;
            }
            else
                return false;
        }

        public ParkingSession ShowActive(string phonenumber)
        {
            foreach (var session in activeSessions)
            if (session.CarPlateNumber == GetCarNumberByPhone(phonenumber))
                {
                    return session;
                }
            return null;
        }

        public List<ParkingSession> ShowPast(string phonenumber)
        {
            List<ParkingSession> pastsessions = new List<ParkingSession>();
            foreach (var session in pastSessions)
                if (session.CarPlateNumber == GetCarNumberByPhone(phonenumber))
                    pastsessions.Add(session);
            return pastsessions;
        }
        public void ChangePassword(string phone, string newpassword)
        {
            foreach(var user in users)
                if(user.Phone == phone)
                {
                    user.Password = newpassword;
                    SaveUserData();
                }
        }

        public decimal FilledSpaces()
        {
            decimal filled = Convert.ToDecimal(activeSessions.Count);
            decimal percent = (filled/parkingCapacity) * 100;
            return percent;
        }

        public List<ParkingSession> Actives()
        {
            List<ParkingSession> act = new List<ParkingSession>();
            act = activeSessions;
            return act;
        }

        public List<ParkingSession> Pasts()
        {
            List<ParkingSession> past = new List<ParkingSession>();
            past = pastSessions;
            return past;
        }

        public decimal? TotalProfit(DateTime start, DateTime end)
        {
            decimal? profit = 0;
            foreach(var session in pastSessions)
                if(session.ExitDt >= start & session.ExitDt <= end)
                    if(session.TotalPayment != null)
                        profit += session.TotalPayment;
            return profit;
        }

        #region Helper methods
        private ParkingSession GetSessionByTicketNumber(int ticketNumber) {
            var session = activeSessions.FirstOrDefault(s => s.TicketNumber == ticketNumber);
            if(session == null)
                throw new ArgumentException($"Session with ticket number = {ticketNumber} does not exist");
            return session;
        }

        private decimal GetCost(double diffInMinutes) {
            var tariff = tariffTable.FirstOrDefault(t => t.Minutes >= diffInMinutes) ?? tariffTable.Last();
            return tariff.Rate;
        }

        private T Deserialize<T>(string fileName) {
            using(var sr = new StreamReader(fileName)) {
                using(var jsonReader = new JsonTextReader(sr)) {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        private void Serialize<T>(string fileName, T data) {
            using(var sw = new StreamWriter(fileName)) {
                using(var jsonWriter = new JsonTextWriter(sw)) {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, data);
                }
            }
        }

        private class ParkingData {
            public List<ParkingSession> PastSessions { get; set; }
            public List<ParkingSession> ActiveSessions { get; set; }
            public int Capacity { get; set; }
        }


        private const string TariffsFileName = "data/tariffs.json";
        private const string ParkingDataFileName = "data/parkingdata.json";
        private const string UsersFileName = "data/users.json";

        private void LoadData() {
            tariffTable = Deserialize<List<Tariff>>(TariffsFileName);
            var data = Deserialize<ParkingData>(ParkingDataFileName);
            users = Deserialize<List<User>>(UsersFileName);

            parkingCapacity = data.Capacity;
            pastSessions = data.PastSessions ?? new List<ParkingSession>();
            activeSessions = data.ActiveSessions ?? new List<ParkingSession>();

            freeLeavePeriod = tariffTable.First().Minutes;
            nextTicketNumber = activeSessions.Count > 0 ? activeSessions.Max(s => s.TicketNumber) + 1 : 1;
        }

        private void SaveData() {
            var data = new ParkingData
            {
                Capacity = parkingCapacity,
                ActiveSessions = activeSessions,
                PastSessions = pastSessions
            };
            Serialize(ParkingDataFileName, data);
        }

        private void SaveUserData()
        {
            List<User> Users = users;
            Serialize(UsersFileName, Users);
        }

        private void CompleteSession(ParkingSession session, DateTime currentDt) {
            session.ExitDt = currentDt;
            activeSessions.Remove(session);
            pastSessions.Add(session);
            SaveData();
        }
        private int CountUserNumber()
        {
            int max = users.Count() + 1;
            return max;
        }
        public string GetCarNumberByPhone(string phonenumber)
        {
            foreach (var user in users)
                if (user.Phone == phonenumber)
                    return user.CarPlateNumber;
            return null;
        }
        #endregion
    }
}
