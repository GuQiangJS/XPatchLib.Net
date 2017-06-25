// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

namespace XPatchLib.UnitTest.TestClass
{
    [PrimaryKey("Name")]
    internal class BookClass
    {
        #region Public Properties

        public AuthorClass Author { get; set; }

        public string Comments { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int PublishYear { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static BookClass GetSampleInstance()
        {
            return new BookClass
            {
                Name = "JERUSALEM:THE BIOGRAPHY",
                Author = AuthorClass.GetSampleInstance(),
                PublishYear = 2015,
                Price = 50.4,
                Comments =
                    @"Jerusalem lies at the centre of the world, the capital of three faiths, the prize of many conquerors,
the jewel of many empires, and the eye of the storm of today's battle of civilisations. But the city
lacks a biography. It lacks a secret history. Simon Sebag Montefiore's epic account is seen through kings,
conquerors, emperors and soldiers; Muslims, Jews, Christians, Macedonians, Romans and Greeks; Palestinians
and Israelis; from King David via Nebuchadnezzar, Alexander the Great, Herod, Caesar, Cleopatra, Jesus and
Saladin, to Churchill, King Hussein, Anwar Sadat and Ariel Sharon. Their individual stories combine to form
the biography of a city - a gritty, dramatic, violent tale of power, empire, love, vanity, luxury and death,
bringing three thousand years of history vividly to life."
            };
        }

        public override bool Equals(object obj)
        {
            var b = obj as BookClass;
            if (b == null)
                return false;
            return string.Equals(Name, b.Name)
                   && string.Equals(Comments, b.Comments)
                   && Equals(Price, b.Price)
                   && Equals(PublishYear, b.PublishYear)
                   && Equals(Author, b.Author);
        }

        #endregion Public Methods
    }
}