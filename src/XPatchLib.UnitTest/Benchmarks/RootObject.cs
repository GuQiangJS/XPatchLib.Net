// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

#if BENCHMARK

using System;
using System.Collections.Generic;

namespace XPatchLib.UnitTest.Benchmarks
{
    [PrimaryKey("guid")]
    public class RootObject
    {
        public static RootObject CreateNew(int tagsCount,int friendsCount)
        {
            RootObject result=new RootObject();

            result.guid = Guid.NewGuid();
            result._id = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.index = BenchmarkHelper.RandomNumber();
            result.isActive = BenchmarkHelper.RandomBoolean();
            result.balance = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.age = BenchmarkHelper.RandomNumber();
            result.eyeColor = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.name = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.gender = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.company = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.email = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.phone = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.address = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.about = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.registered=BenchmarkHelper.RandomDate();
            result.latitude = BenchmarkHelper.RandomDoubleNumber();
            result.longitude = (decimal)BenchmarkHelper.RandomDoubleNumber();
            result.greeting = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.favoriteFruit = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);

            result.friends = new List<Friend>(friendsCount);
            for (int i = 0; i < friendsCount; i++)
            {
                result.friends.Add(Friend.CreateNew());
            }

            result.tags = new List<string>(tagsCount);
            for (int i = 0; i < tagsCount; i++)
            {
                result.tags.Add(BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty));
            }

            return result;
        }



        public string _id { get; set; }
        public int index { get; set; }
        public Guid guid { get; set; }
        public bool isActive { get; set; }
        public string balance { get; set; }
        //public Uri picture { get; set; }
        public int age { get; set; }
        public string eyeColor { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string about { get; set; }
        public DateTime registered { get; set; }
        public double latitude { get; set; }
        public decimal longitude { get; set; }
        public List<string> tags { get; set; }
        public List<Friend> friends { get; set; }
        public string greeting { get; set; }
        public string favoriteFruit { get; set; }
    }

    [PrimaryKey("guid")]
    public class Friend
    {
        public static Friend CreateNew()
        {
            Friend result = new Friend();
            result.name = BenchmarkHelper.RandomString(10, true, true, true, true, string.Empty);
            result.id = BenchmarkHelper.RandomNumber();
            result.guid=Guid.NewGuid();
            return result;
        }

        public Guid guid { get; set; }

        public int id { get; set; }
        public string name { get; set; }
    }
}
#endif