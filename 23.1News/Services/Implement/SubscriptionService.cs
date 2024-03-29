﻿using _23._1News.Data;
using _23._1News.Models.Db;
using _23._1News.Models.View_Models;
using _23._1News.Models.ViewModels;
using _23._1News.Services.Abstract;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System;
using System.Diagnostics;

namespace _23._1News.Services.Implement
{
    public class SubscriptionService: ISubscriptionService
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public SubscriptionService(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        
    }
 
        public List<Subscription> GetAllSubs()
        {
            return _db.Subscriptions.ToList();
        }

        public void CreateSubs(Subscription newSub)
        {
            newSub.SubscriptionType = _db.SubscriptionTypes.Where(t => t.Id == newSub.SubscriptionTypeId).FirstOrDefault()!;
            newSub.Created = DateTime.Now;
            newSub.Price = newSub.SubscriptionType.Price;
            newSub.IsActive = true;
            _db.Subscriptions.Add(newSub);
            _db.SaveChanges();
        }

        public bool UpdateSubs(Subscription newSubs) 
        {
            try
            {
                _db.Subscriptions.Update(newSubs);
                _db.SaveChanges();
                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public Subscription GetSubsById(int id)
        {
            return _db.Subscriptions.Find(id);
        }

        public User GetUserById(string id)
        {

            return _db.Users.Find(id);


        }

        public List<Subscription> GetSubsByUserId(string id)
        {
            var subscriptions = _db.Subscriptions.Where(Subscription => Subscription.UserId== id)
                            .OrderByDescending(a => a.Created).ToList();

            return subscriptions;
        }

        public bool DeleteSubs(int id)
        {
            try
            {
                var sub = this.GetSubsById(id);
                if (sub == null)
                {
                    return false;
                }
                _db.Subscriptions.Remove(sub);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public int GetActiveSubscribersCount()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            return _db.Subscriptions.Count(s => s.Created >= thirtyDaysAgo);
        }


        public Subscription GetActiveSubscriptionByUser(string userId)
        {
            // Retrieve the active subscription for the given user
            var activeSubscription = _db.Subscriptions
                .Where(s => s.User.Id == userId && s.IsActive)
                .OrderByDescending(s => s.Created)
                .FirstOrDefault();

            return activeSubscription;
        }

        public IEnumerable<Subscription> GetWeeklySubscriptionData()
        {
            var weeklyData = _db.WeeklySubscriptionData.ToList();

            var subscriptions = weeklyData.Select(weekly =>
                new Subscription
                {
                    // Map properties from WeeklySubscriptionData to Subscription
                    // For example:
                    // Id = weekly.Id,
                    // SubscriptionType = weekly.SubscriptionType,
                    // Price = weekly.Price,
                    // ...

                    // Assign properties specific to Subscription, adapt this to your needs
                    //WeekLabel = weekly.WeekLabel,
                    SubscriberCount = weekly.SubscriberCount,
                    // ...
                });

            return subscriptions;
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            // Assuming SubscriptionTypes is a DbSet<SubscriptionType> in your DbContext
            return _db.SubscriptionTypes.ToList();
        }

        //object ISubscriptionService.GetSubsByUserId(string? userId)
        //{
        //    throw new NotImplementedException();
        //}
        
        public bool isEnteprise(string userId)
        {
            // Retrieve the active subscription for the user
            var activeSubscription = GetActiveSubscriptionByUserId(userId);

            //Check if the user has an active subscription of the specified type
            //return activeSubscription != null &&
            //       activeSubscription.SubscriptionType?.TypeName == "Enterprise";
            return activeSubscription != null &&
                   activeSubscription.SubscriptionTypeId == 11;
        }

        public Subscription GetActiveSubscriptionByUserId(string userId)
        {
            // Retrieve the active subscription for the given user
            var activeSubscription = _db.Subscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderByDescending(s => s.Created)
                .FirstOrDefault();

            return activeSubscription;
        }

        // ... other methods ...
    }

    }


